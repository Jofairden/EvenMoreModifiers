using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.AccessoryModifiers
{
	public class Inferno : AccessoryModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = "Player has inferno", Color =  Color.IndianRed},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(rollChance: 0f, rarityLevel: 3f);
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			player.inferno = true;
		}
	}
}
