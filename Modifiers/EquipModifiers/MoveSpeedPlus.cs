using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class MoveSpeedPlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower}% movement speed", Color =  Color.LimeGreen},
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 10f;

		public override void UpdateEquip(Item item, Player player)
		{
			player.moveSpeed += RoundedPower / 100;
			player.maxRunSpeed *= 1 + RoundedPower / 100;
		}
	}
}
