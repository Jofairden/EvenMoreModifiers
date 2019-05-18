using System;
using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Utility
{
	public class WingTimePlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Math.Round(Properties.RoundedPower / 60f, 2)}s flight time");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(60f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.wingTimeMax += (int) Properties.RoundedPower;
		}
	}
}
