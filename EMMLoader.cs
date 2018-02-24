using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Loot.Modifiers
{
	internal struct RarityMap
	{
		public string Name;
		public ModifierRarity Rarity;
	}

	internal struct EffectMap
	{
		public string Name;
		public ModifierEffect Effect;
	}

	internal struct ModifierMap
	{
		public string Name;
		public Modifier Modifier;
	}

	/// <summary>
	/// The loader, handles all loading
	/// </summary>
	public static class EMMLoader
	{
		private static uint rarityNextID;
		private static uint effectNextID;
		private static uint modifierNextID;
		internal static IDictionary<string, List<RarityMap?>> RaritiesMap;
		internal static IDictionary<string, List<EffectMap?>> EffectsMap;
		internal static IDictionary<string, List<ModifierMap?>> ModifiersMap;
		internal static IDictionary<uint, ModifierRarity> Rarities;
		internal static IDictionary<uint, ModifierEffect> Effects;
		internal static IDictionary<uint, Modifier> Modifiers;
		internal static IDictionary<string, Assembly> Mods;

		internal static void Initialize()
		{
			rarityNextID = 0;
			effectNextID = 0;
			modifierNextID = 0;
			RaritiesMap = new Dictionary<string, List<RarityMap?>>();
			EffectsMap = new Dictionary<string, List<EffectMap?>>();
			ModifiersMap = new Dictionary<string, List<ModifierMap?>>();
			Rarities = new Dictionary<uint, ModifierRarity>();
			Effects = new Dictionary<uint, ModifierEffect>();
			Modifiers = new Dictionary<uint, Modifier>();
			Mods = new ConcurrentDictionary<string, Assembly>();
		}

		internal static void Load()
		{
			RegisterMod(Loot.Instance);
		}

		internal static void Unload()
		{
			rarityNextID = 0;
			effectNextID = 0;
			modifierNextID = 0;
			RaritiesMap = null;
			EffectsMap = null;
			ModifiersMap = null;
			Rarities = null;
			Effects = null;
			Modifiers = null;
			Mods = null;
		}

		/// <summary>
		/// Registers specified mod, enabling autoloading for that mod
		/// </summary>
		/// <param name="mod"></param>
		public static void RegisterMod(Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("RegisterMod can only be called from Mod.Load or Mod.Autoload");
			}

			if (Mods.ContainsKey(mod.Name))
			{
				throw new Exception($"Mod {mod.Name} is already registered");
			}

			Mods.Add(new KeyValuePair<string, Assembly>(mod.Name, mod.Code));
			RaritiesMap.Add(new KeyValuePair<string, List<RarityMap?>>(mod.Name, new List<RarityMap?>()));
			EffectsMap.Add(new KeyValuePair<string, List<EffectMap?>>(mod.Name, new List<EffectMap?>()));
			ModifiersMap.Add(new KeyValuePair<string, List<ModifierMap?>>(mod.Name, new List<ModifierMap?>()));
		}

		internal static uint ReserveRarityID()
		{
			uint reserved = rarityNextID;
			rarityNextID++;
			return reserved;
		}

		internal static uint ReserveEffectID()
		{
			uint reserved = effectNextID;
			effectNextID++;
			return reserved;
		}

		internal static uint ReserveModifierID()
		{
			uint reserved = modifierNextID;
			modifierNextID++;
			return reserved;
		}

		internal static Modifier GetWeightedModifier(ModifierContext ctx)
		{
			var wr = new WeightedRandom<Modifier>();
			foreach (var m in Modifiers.Where(x => x.Value._CanApply(ctx)))
				wr.Add(m.Value, m.Value.RollChance);
			var mod = wr.Get();
			return (Modifier)mod?.Clone();
		}

		internal static ModifierRarity GetModifierRarity(Modifier modifier)
		{
			return (ModifierRarity)Rarities
					.Select(r => r.Value)
					.OrderByDescending(r => r.RequiredRarityLevel)
					.FirstOrDefault(r => r.MatchesRequirements(modifier))
					.Clone();
		}

		/// <summary>
		/// Returns the ModifierRarity specified by type, null if not present
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ModifierRarity GetRarity(uint type)
		{
			return type < rarityNextID ? (ModifierRarity)Rarities[type].Clone() : null;
		}

		/// <summary>
		/// Returns the ModifierEffect specified by type, null if not present
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ModifierEffect GetEffect(uint type)
		{
			return type < effectNextID ? (ModifierEffect)Effects[type].Clone() : null;
		}

		/// <summary>
		/// Returns the Modifier specified by type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Modifier GetModifier(uint type)
		{
			return type < modifierNextID ? (Modifier)Modifiers[type].Clone() : null;
		}

		/// <summary>
		/// Will iterate registered mods, and attempt to autoload their rarities and effects
		/// </summary>
		internal static void SetupContent()
		{
			foreach (var kvp in Mods)
			{
				var ordered = kvp.Value
					.GetTypes()
					.OrderBy(x => x.FullName, StringComparer.InvariantCulture)
					.Where(t => t.IsClass && !t.IsAbstract); /* || type.GetConstructor(new Type[0]) == null*/

				var rarities = ordered.Where(x => x.IsSubclassOf(typeof(ModifierRarity)));
				var effects = ordered.Where(x => x.IsSubclassOf(typeof(ModifierEffect)));
				var modifiers = ordered.Where(x => x.IsSubclassOf(typeof(Modifier)));

				// important: load things in order. (modifiers relies on all.. etc.)
				foreach (Type type in rarities)
				{
					AutoloadRarity(type, ModLoader.GetMod(kvp.Key));
				}

				foreach (Type type in effects)
				{
					AutoloadEffect(type, ModLoader.GetMod(kvp.Key));
				}

				foreach (Type type in modifiers)
				{
					AutoloadModifier(type, ModLoader.GetMod(kvp.Key));
				}
			}

			//ErrorLogger.ClearLog();
			//ErrorLogger.Log(string.Join("\n", Rarities.Select(r => r.Value.Name)));
			//ErrorLogger.Log(string.Join("\n", Effects.Select(e => e.Value.Description)));
		}

		private static void AutoloadModifier(Type type, Mod mod)
		{
			Modifier modifier = (Modifier)Activator.CreateInstance(type);
			AddModifier(modifier, mod);
		}

		public static void AddModifier(Modifier modifier, Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("AddModifier can only be called from Mod.Load or Mod.Autoload");
			}
			if (!Mods.ContainsKey(mod.Name))
			{
				throw new Exception($"Mod {mod.Name} is not registered, please register before adding");
			}

			List<ModifierMap?> lmm;
			if (!ModifiersMap.TryGetValue(mod.Name, out lmm))
			{
				throw new Exception($"ModifierMaps for {mod.Name} not found");
			}
			else if (lmm.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(modifier.Name)) != null)
			{
				throw new Exception($"You have already added a modifier with the name {modifier.Name}");
			}

			modifier.Mod = mod;
			modifier.Type = ReserveModifierID();
			Modifiers[modifier.Type] = modifier;
			ModifiersMap[mod.Name].Add(new ModifierMap { Name = modifier.Name, Modifier = modifier });
		}

		/// <summary>
		/// Autoloads a rarity
		/// </summary>
		/// <param name="type"></param>
		/// <param name="mod"></param>
		private static void AutoloadRarity(Type type, Mod mod)
		{
			ModifierRarity rarity = (ModifierRarity)Activator.CreateInstance(type);
			AddRarity(rarity, mod);
		}

		/// <summary>
		/// Adds a rarity
		/// </summary>
		/// <param name="rarity"></param>
		/// <param name="mod"></param>
		public static void AddRarity(ModifierRarity rarity, Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("AddRarity can only be called from Mod.Load or Mod.Autoload");
			}
			if (!Mods.ContainsKey(mod.Name))
			{
				throw new Exception($"Mod {mod.Name} is not registered, please register before adding");
			}

			List<RarityMap?> lrm;
			if (!RaritiesMap.TryGetValue(mod.Name, out lrm))
			{
				throw new Exception($"RarityMaps for {mod.Name} not found");
			}
			else if (lrm.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(rarity.Name)) != null)
			{
				throw new Exception($"You have already added a rarity with the name {rarity.Name}");
			}

			rarity.Mod = mod;
			rarity.Type = ReserveRarityID();
			Rarities[rarity.Type] = rarity;
			RaritiesMap[mod.Name].Add(new RarityMap { Name = rarity.Name, Rarity = rarity });
		}

		/// <summary>
		/// Autoloads an effect
		/// </summary>
		/// <param name="type"></param>
		/// <param name="mod"></param>
		private static void AutoloadEffect(Type type, Mod mod)
		{
			ModifierEffect effect = (ModifierEffect)Activator.CreateInstance(type);
			AddEffect(effect, mod);
		}

		/// <summary>
		/// Adds an effect
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="mod"></param>
		public static void AddEffect(ModifierEffect effect, Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("AddEffect can only be called from Mod.Load or Mod.Autoload");
			}

			List<EffectMap?> lem;
			if (!EffectsMap.TryGetValue(mod.Name, out lem))
			{
				throw new Exception($"EffectMaps for {mod.Name} not found");
			}
			else if (lem.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(effect.Name)) != null)
			{
				throw new Exception($"You have already added a effect with the name {effect.Name}");
			}

			effect.Mod = mod;
			effect.Type = ReserveEffectID();
			Effects[effect.Type] = effect;
			EffectsMap[mod.Name].Add(new EffectMap { Name = effect.Name, Effect = effect });
		}

		internal static Exception ThrowException(string message)
			=> new Exception($"{Loot.Instance.DisplayName ?? "EvenMoreModifiers"}: {message}");

		/// <summary>
		/// Requests all effects, and returns them as a readonly collection
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyCollection<ModifierEffect> RequestEffects()
		{
			return Effects.Select(e => (ModifierEffect)e.Value?.Clone()).ToList().AsReadOnly();
		}

		/// <summary>
		/// Requests all rarities, and returns them as a readonly collection
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyCollection<ModifierRarity> RequestRarities()
		{
			return Rarities.Select(r => (ModifierRarity)r.Value?.Clone()).ToList().AsReadOnly();
		}

		/// <summary>
		/// Requests all modifiers, and returns them as a readonly collection
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyCollection<Modifier> RequestModifiers()
		{
			return Modifiers.Select(m => (Modifier)m.Value?.Clone()).ToList().AsReadOnly();
		}

	}
}
