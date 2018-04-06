using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class MagicDamagePlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower}% magic damage", Color =  Color.LimeGreen},
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 8f;
		
		public override void UpdateEquip(Item item, Player player)
		{
			player.magicDamage += RoundedPower / 100;
		}
	}
}
