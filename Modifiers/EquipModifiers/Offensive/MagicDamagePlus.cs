using Loot.Core.System;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class MagicDamagePlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% magic damage");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.magicDamage += Properties.RoundedPower / 100;
		}
	}
}
