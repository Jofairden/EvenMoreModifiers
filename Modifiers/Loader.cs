using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	/// <summary>
	/// The modifier loader, handles modifier and rarity loading
	/// </summary>
	public static class Loader
	{
		//public static class OurRarities
		//{
		//	public static ModifierRarity Common = new ModifierRarity("Common", 0f, Color.White, RequirementPass.MatchStrengthPass);
		//	public static ModifierRarity Uncummon = new ModifierRarity("Uncummon", 1f, Color.Orange, RequirementPass.MatchStrengthPass);
		//	public static ModifierRarity Rare = new ModifierRarity("Rare", 2f, Color.Yellow, RequirementPass.MatchStrengthPass);
		//	public static ModifierRarity Legendary = new ModifierRarity("Legendary", 4f, Color.Red, RequirementPass.MatchStrengthPass);
		//	public static ModifierRarity Transcendent = new ModifierRarity("Transcendent", 8f, Color.Purple, RequirementPass.MatchStrengthPass);
		//}

		private static ushort rarityNextID = 0;
		private static ushort effectNextID = 0;
		internal static IDictionary<ushort, ModifierRarity> Rarities = new Dictionary<ushort, ModifierRarity>();
		internal static IDictionary<ushort, ModifierEffect> Effects = new Dictionary<ushort, ModifierEffect>();

		internal static IDictionary<string, Assembly> Mods = new ConcurrentDictionary<string, Assembly>();

		internal static void Load()
		{
			RegisterMod(Loot.Instance);
		}

		internal static void Unload()
		{
			Rarities = null;
			Effects = null;
			Mods = null;
		}

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
		}

		internal static ushort ReserveRarityID()
		{
			ushort reserved = rarityNextID;
			rarityNextID++;
			return reserved;
		}

		internal static ushort ReserveEffectID()
		{
			ushort reserved = effectNextID;
			effectNextID++;
			return reserved;
		}

		public static ModifierRarity GetRarity(ushort type)
		{
			return type < rarityNextID ? Rarities[type] : null;
		}

		public static ModifierEffect GetEffect(ushort type)
		{
			return type < effectNextID ? Effects[type] : null;
		}

		internal static void SetupContent()
		{
			//public static ModifierRarity Common = new ModifierRarity("Common", 0f, Color.White);
			//public static ModifierRarity Uncommon = new ModifierRarity("Uncommon", 1f, Color.Orange);
			//public static ModifierRarity Rare = new ModifierRarity("Rare", 2f, Color.Yellow);
			//public static ModifierRarity Legendary = new ModifierRarity("Legendary", 4f, Color.Red);
			//public static ModifierRarity Transcendent = new ModifierRarity("Transcendent", 8f, Color.Purple);

			foreach (var kvp in Mods)
			{
				foreach (Type type in kvp.Value.GetTypes().OrderBy(x => x.FullName, StringComparer.InvariantCulture))
				{
					if (type.IsAbstract || type.GetConstructor(new Type[0]) == null)
					{
						continue;
					}

					if (type.IsSubclassOf(typeof(ModifierRarity)))
					{
						AutoloadRarity(type, ModLoader.GetMod(kvp.Key));
					}
					else if (type.IsSubclassOf(typeof(ModifierEffect)))
					{
						AutoloadEffect(type, ModLoader.GetMod(kvp.Key));
					}
				}
			}

			//ErrorLogger.ClearLog();
			//ErrorLogger.Log(string.Join("\n", Rarities.Select(r => r.Value.Name)));
			//ErrorLogger.Log(string.Join("\n", Effects.Select(e => e.Value.Description)));
		}

		private static void AutoloadRarity(Type type, Mod mod)
		{
			ModifierRarity rarity = (ModifierRarity)Activator.CreateInstance(type);
			AddRarity(rarity, mod);
		}

		public static void AddRarity(ModifierRarity rarity, Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("AddRarity can only be called from Mod.Load or Mod.Autoload");
			}

			rarity.Mod = mod;
			rarity.Type = ReserveRarityID();
			Rarities[rarity.Type] = rarity;
		}

		private static void AutoloadEffect(Type type, Mod mod)
		{
			ModifierEffect effect = (ModifierEffect)Activator.CreateInstance(type);
			AddEffect(effect, mod);
		}

		public static void AddEffect(ModifierEffect effect, Mod mod)
		{
			bool? b = mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) as bool?;
			if (b != null && !b.Value)
			{
				throw new Exception("AddEffect can only be called from Mod.Load or Mod.Autoload");
			}

			effect.Mod = mod;
			effect.Type = ReserveEffectID();
			Effects[effect.Type] = effect;
		}

		internal static Exception ThrowException(string message)
			=> new Exception($"{Loot.Instance.DisplayName ?? "EvenMoreModifiers"}: {message}");

		public static IReadOnlyCollection<ModifierEffect> RequestEffects()
		{
			return Effects.Select(e => e.Value).ToList().AsReadOnly();
		}

		public static IReadOnlyCollection<ModifierRarity> RequestRarities()
		{
			return Rarities.Select(r => r.Value).ToList().AsReadOnly();
		}
	}
}
