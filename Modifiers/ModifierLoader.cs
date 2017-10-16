using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Loot.Modifiers
{
	/// <summary>
	/// The modifier loader, handles modifier and rarity loading
	/// </summary>
	public static class ModifierLoader
	{
		public static class OurRarities
		{
			public static ModifierRarity Common = new ModifierRarity("Common", 0f, Color.White, RequirementPass.MatchStrengthPass);
			public static ModifierRarity Uncummon = new ModifierRarity("Uncummon", 1f, Color.Orange, RequirementPass.MatchStrengthPass);
			public static ModifierRarity Rare = new ModifierRarity("Rare", 2f, Color.Yellow, RequirementPass.MatchStrengthPass);
			public static ModifierRarity Legendary = new ModifierRarity("Legendary", 4f, Color.Red, RequirementPass.MatchStrengthPass);
			public static ModifierRarity Transcendent = new ModifierRarity("Transcendent", 8f, Color.Purple, RequirementPass.MatchStrengthPass);
		}

		internal static IList<ModifierRarity> Rarities { get; private set; }
		internal static IList<Modifier> Modifiers { get; private set; }

		internal static void Load()
		{
			Rarities = new List<ModifierRarity>();
			Modifiers = new List<Modifier>();
		}

		internal static void Unload()
		{
			Rarities = null;
			Modifiers = null;
		}

		internal static void SetupContent()
		{
			//public static ModifierRarity Common = new ModifierRarity("Common", 0f, Color.White);
			//public static ModifierRarity Uncommon = new ModifierRarity("Uncommon", 1f, Color.Orange);
			//public static ModifierRarity Rare = new ModifierRarity("Rare", 2f, Color.Yellow);
			//public static ModifierRarity Legendary = new ModifierRarity("Legendary", 4f, Color.Red);
			//public static ModifierRarity Transcendent = new ModifierRarity("Transcendent", 8f, Color.Purple);
			var _rarities = new[]
			{
				OurRarities.Common,
				OurRarities.Uncummon,
				OurRarities.Rare,
				OurRarities.Legendary,
				OurRarities.Transcendent
			};

			var _modifiers = new[]
			{
				new ItemModifier("Debug test", "Test description")
			};

			_rarities.ToList().ForEach(rarity =>
			{
				AddRarity(rarity);
			});

			_modifiers.ToList().ForEach(modifier =>
			{
				AddModifier(modifier);
			});
		}

		public static bool AddRarity(ModifierRarity rarity)
		{
			if (!Rarities.Any(r => r.Name.Equals(rarity.Name, StringComparison.InvariantCultureIgnoreCase)))
			{
				Rarities.Add(rarity);
				return true;
			}
			return false;
		}

		public static bool AddModifier(Modifier modifier)
		{
			if (!Modifiers.Any(m => m.Name.Equals(modifier.Name, StringComparison.InvariantCultureIgnoreCase)))
			{
				Modifiers.Add(modifier);
				return true;
			}
			return false;
		}

		public static ModifierRarity GetRarity(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw ThrowException("Rarity was requested but no name was given");

			return Rarities.FirstOrDefault(rarity => name.Equals(rarity.Name, StringComparison.InvariantCultureIgnoreCase));
		}

		public static Modifier GetModifier(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw ThrowException("Modifier was requested but no name was given");

			return Modifiers.FirstOrDefault(modifier => name.Equals(modifier.Name, StringComparison.InvariantCultureIgnoreCase));
		}

		internal static Exception ThrowException(string message)
			=> new Exception($"{Loot.Instance.DisplayName ?? "EvenMoreModifiers"}: {message}");
	}
}
