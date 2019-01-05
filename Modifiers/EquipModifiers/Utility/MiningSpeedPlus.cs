using Loot.Core.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Utility
{
	public class MiningSpeedPlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% mining speed", Color =  Color.LimeGreen},
		};

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(7.5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.pickSpeed -= (player.pickSpeed * Properties.RoundedPower / 100);
		}
	}
}
