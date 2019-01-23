using Loot.Core.System;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class MagicCritPlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% magic crit chance");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.magicCrit += (int) Properties.RoundedPower;
		}
	}
}
