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

		public override float MinMagnitude => 1f;
		public override float MaxMagnitude => 10f;

		public override void UpdateEquip(Item item, Player player)
		{
			player.moveSpeed += RoundedPower / 100;
			player.maxRunSpeed += RoundedPower / 100;
		}
	}
}
