using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class MiningSpeedPlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% mining speed", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 10f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.pickSpeed -= (player.pickSpeed * Properties.RoundedPower / 100);
		}
	}
}
