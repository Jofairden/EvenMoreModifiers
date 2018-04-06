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

		public override float GetRarityLevel(Item item) => 3f;

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			player.inferno = true;
		}
	}
}
