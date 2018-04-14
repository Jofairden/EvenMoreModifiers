using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loot.System;
using Terraria.ModLoader;
using Terraria.Utilities;

using RarityMap = System.Collections.Generic.KeyValuePair<string, Loot.System.ModifierRarity>;
using ModifierMap = System.Collections.Generic.KeyValuePair<string, Loot.System.Modifier>;
using PoolMap = System.Collections.Generic.KeyValuePair<string, Loot.System.ModifierPool>;

namespace Loot
{
	/// <summary>
	/// The loader, handles all loading
	/// </summary>
	public static class EMMLoader
	{
		private static uint rarityNextID;
		private static uint modifierNextID;
		private static uint poolNextID;
		internal static IDictionary<string, List<RarityMap>> RaritiesMap;
		internal static IDictionary<string, List<ModifierMap>> ModifiersMap;
		internal static IDictionary<string, List<PoolMap>> PoolsMap;
		internal static IDictionary<uint, ModifierRarity> Rarities;
		internal static IDictionary<uint, Modifier> Modifiers;
		internal static IDictionary<uint, ModifierPool> Pools;
		internal static IDictionary<string, Assembly> Mods;

		internal static void Initialize()
		{
			rarityNextID = 0;
			modifierNextID = 0;
			poolNextID = 0;
			RaritiesMap = new Dictionary<string, List<RarityMap>>();
			ModifiersMap = new Dictionary<string, List<ModifierMap>>();
			PoolsMap = new Dictionary<string, List<PoolMap>>();
			Rarities = new Dictionary<uint, ModifierRarity>();
			Modifiers = new Dictionary<uint, Modifier>();
			Pools = new Dictionary<uint, ModifierPool>();
			Mods = new ConcurrentDictionary<string, Assembly>();
		}

		internal static void Load()
		{
			
		}

		internal static void Unload()
		{
			rarityNextID = 0;
			modifierNextID = 0;
			poolNextID = 0;
			RaritiesMap = null;
			ModifiersMap = null;
			PoolsMap = null;
			Rarities = null;
			Modifiers = null;
			Pools = null;
			Mods = null;
		}

		/// <summary>
		/// Registers specified mod, enabling autoloading for that mod
		/// </summary>
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

			Assembly code;
#if DEBUG
			code = Assembly.GetAssembly(mod.GetType());
#else
			code = mod.Code;
#endif

			Mods.Add(new KeyValuePair<string, Assembly>(mod.Name, code));
			RaritiesMap.Add(new KeyValuePair<string, List<RarityMap>>(mod.Name, new List<RarityMap>()));
			ModifiersMap.Add(new KeyValuePair<string, List<ModifierMap>>(mod.Name, new List<ModifierMap>()));
			PoolsMap.Add(new KeyValuePair<string, List<PoolMap>>(mod.Name, new List<PoolMap>()));
		}

		internal static uint ReserveRarityID()
		{
			uint reserved = rarityNextID;
			rarityNextID++;
			return reserved;
		}

		internal static uint ReserveModifierID()
		{
			uint reserved = modifierNextID;
			modifierNextID++;
			return reserved;
		}

		internal static uint ReservePoolID()
		{
			uint reserved = poolNextID;
			poolNextID++;
			return reserved;
		}

		// Returns a random weighted pool from all available pools that can apply
		internal static ModifierPool GetWeightedPool(ModifierContext ctx)
		{
			var wr = new WeightedRandom<ModifierPool>();
			foreach (var m in Pools.Where(x => x.Value._CanApply(ctx)))
				wr.Add(m.Value, m.Value.RollChance);
			var mod = wr.Get();
			return (ModifierPool)mod?.Clone();
		}

		// Returns the rarity of a pool
		internal static ModifierRarity GetPoolRarity(ModifierPool modifierPool)
		{
			return (ModifierRarity)Rarities
					.Select(r => r.Value)
					.OrderByDescending(r => r.RequiredRarityLevel)
					.FirstOrDefault(r => r.MatchesRequirements(modifierPool))
					?.Clone();
		}

		/// <summary>
		/// Returns the ModifierRarity specified by type, null if not present
		/// </summary>
		public static ModifierRarity GetModifierRarity(uint type)
		{
			return type < rarityNextID ? (ModifierRarity)Rarities[type].Clone() : null;
		}

		/// <summary>
		/// Returns the ModifierEffect specified by type, null if not present
		/// </summary>
		public static Modifier GetModifier(uint type)
		{
			return type < modifierNextID ? (Modifier)Modifiers[type].Clone() : null;
		}

		/// <summary>
		/// Returns the Modifier specified by type, null if not present
		/// </summary>
		public static ModifierPool GetModifierPool(uint type)
		{
			return type < poolNextID ? (ModifierPool)Pools[type].Clone() : null;
		}


		/// <summary>
		/// Sets up content for the specified mod
		/// </summary>
		internal static void SetupContent(Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("SetupContent for EMMLoader can only be called from Mod.Load or Mod.Autoload");
			}

			if (!Mods.ContainsKey(mod.Name))
			{
				throw new Exception($"Mod {mod.Name} is not yet registered in EMMLoader");
			}

			var ordered = Mods.FirstOrDefault(x => x.Key.Equals(mod.Name))
				.Value
				.GetTypes()
				.OrderBy(x => x.FullName, StringComparer.InvariantCulture)
				.Where(t => t.IsClass && !t.IsAbstract); /* || type.GetConstructor(new Type[0]) == null*/

			var rarities = ordered.Where(x => x.IsSubclassOf(typeof(ModifierRarity)));
			var modifiers = ordered.Where(x => x.IsSubclassOf(typeof(Modifier)));
			var pools = ordered.Where(x => x.IsSubclassOf(typeof(ModifierPool)));

			// important: load things in order. (modifiers relies on all.. etc.)
			foreach (Type type in rarities)
			{
				AutoloadModifierRarity(type, mod);
			}

			foreach (Type type in modifiers)
			{
				AutoloadModifier(type, mod);
			}

			foreach (Type type in pools)
			{
				AutoloadModifierPool(type, mod);
			}
		}

		private static void AutoloadModifierPool(Type type, Mod mod)
		{
			ModifierPool modifierPool = (ModifierPool)Activator.CreateInstance(type);
			AddModifierPool(modifierPool, mod);
		}

		public static void AddModifierPool(ModifierPool pool, Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("AddModifierPool can only be called from Mod.Load or Mod.Autoload");
			}
			if (!Mods.ContainsKey(mod.Name))
			{
				throw new Exception($"Mod {mod.Name} is not registered, please register before adding");
			}

			List<PoolMap> lmm;
			if (!PoolsMap.TryGetValue(mod.Name, out lmm))
			{
				throw new Exception($"PoolMaps for {mod.Name} not found");
			}

			if (lmm.Exists(x => x.Value.Name.Equals(pool.Name)))
			{
				throw new Exception($"You have already added a ModifierPool with the name {pool.Name}");
			}

			pool.Mod = mod;
			pool.Type = ReservePoolID();
			Pools[pool.Type] = pool;
			PoolsMap[mod.Name].Add(new PoolMap(pool.Name, pool));
		}

		/// <summary>
		/// Autoloads a rarity
		/// </summary>
		private static void AutoloadModifierRarity(Type type, Mod mod)
		{
			ModifierRarity rarity = (ModifierRarity)Activator.CreateInstance(type);
			AddModifierRarity(rarity, mod);
		}

		/// <summary>
		/// Adds a rarity
		/// </summary>
		public static void AddModifierRarity(ModifierRarity rarity, Mod mod)
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

			List<RarityMap> lrm;
			if (!RaritiesMap.TryGetValue(mod.Name, out lrm))
			{
				throw new Exception($"RarityMaps for {mod.Name} not found");
			}

			if (lrm.Exists(x => x.Value.Name.Equals(rarity.Name)))
			{
				throw new Exception($"You have already added a rarity with the name {rarity.Name}");
			}

			rarity.Mod = mod;
			rarity.Type = ReserveRarityID();
			Rarities[rarity.Type] = rarity;
			RaritiesMap[mod.Name].Add(new RarityMap(rarity.Name, rarity));
		}

		/// <summary>
		/// Autoloads a modifier
		/// </summary>
		private static void AutoloadModifier(Type type, Mod mod)
		{
			Modifier modifier = (Modifier)Activator.CreateInstance(type);
			AddModifier(modifier, mod);
		}

		/// <summary>
		/// Adds a modifier
		/// </summary>
		public static void AddModifier(Modifier modifier, Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("AddModifier can only be called from Mod.Load or Mod.Autoload");
			}

			List<ModifierMap> lem;
			if (!ModifiersMap.TryGetValue(mod.Name, out lem))
			{
				throw new Exception($"ModifierMaps for {mod.Name} not found");
			}

			if (lem.Exists(x => x.Value.Name.Equals(modifier.Name)))
			{
				throw new Exception($"You have already added a modifier with the name {modifier.Name}");
			}

			modifier.Mod = mod;
			modifier.Type = ReserveModifierID();
			Modifiers[modifier.Type] = modifier;
			ModifiersMap[mod.Name].Add(new ModifierMap(modifier.Name, modifier));
		}

		internal static Exception ThrowException(string message)
			=> new Exception($"{Loot.Instance.DisplayName ?? "EvenMoreModifiers"}: {message}");

		/// <summary>
		/// Requests all effects, and returns them as a readonly collection
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyCollection<Modifier> RequestModifiers()
			=> Modifiers.Select(e => (Modifier)e.Value?.Clone()).ToList().AsReadOnly();

		/// <summary>
		/// Requests all rarities, and returns them as a readonly collection
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyCollection<ModifierRarity> RequestModifierRarities()
			=> Rarities.Select(r => (ModifierRarity)r.Value?.Clone()).ToList().AsReadOnly();

		/// <summary>
		/// Requests all modifiers, and returns them as a readonly collection
		/// </summary>
		/// <returns></returns>
		public static IReadOnlyCollection<ModifierPool> RequestModifierPools()
			=> Pools.Select(m => (ModifierPool)m.Value?.Clone()).ToList().AsReadOnly();
	}
}
