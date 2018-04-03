using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class MeleeDamagePlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower}% melee damage", Color =  Color.LimeGreen},
		};

		public override float MinMagnitude => 1f;
		public override float MaxMagnitude => 8f;

		public override void UpdateEquip(Item item, Player player)
		{
			player.meleeDamage += RoundedPower / 100;
		}
	}
}
