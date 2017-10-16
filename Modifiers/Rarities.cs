using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Modifiers
{
	/// <summary>
	/// Handles rarity of modifiers globally, mainly a helper class
	/// </summary>
	public static class Rarities
	{
		public static Modifier.ModifierRarity GetRarityByColor(Modifier.RarityColor color)
			=> (Modifier.ModifierRarity)color;

		public static Modifier.RarityColor GetColorByRarity(Modifier.ModifierRarity rarity)
			=> (Modifier.RarityColor)rarity;
	}
}
