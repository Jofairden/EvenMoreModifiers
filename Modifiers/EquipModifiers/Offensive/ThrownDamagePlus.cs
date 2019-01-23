using Loot.Core.System;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class ThrownDamagePlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% thrown damage");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.thrownDamage += Properties.RoundedPower / 100;
		}
	}
}
