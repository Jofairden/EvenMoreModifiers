using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class MiningSpeedPlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower}% mining speed", Color =  Color.LimeGreen},
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 10f;
		
		public override void UpdateEquip(Item item, Player player)
		{
			player.pickSpeed *= RoundedPower / 100;
		}
	}
}
