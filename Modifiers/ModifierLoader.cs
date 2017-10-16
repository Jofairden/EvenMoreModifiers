using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Modifiers
{
	// @todo: honestly, is this needed?
	/// <summary>
	/// The modifier loader
	/// </summary>
	public static class ModifierLoader
	{
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
			// Create a collection of modifiers
			var _modifiers = new[]
			{
				new ItemModifier("Debug test")
			};

			// Add the modifiers
			_modifiers.ToList().ForEach(modifier =>
			{
				// @todo: require unique name?
				if (!Modifiers.Contains(modifier))
					Modifiers.Add(modifier);
			});
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
