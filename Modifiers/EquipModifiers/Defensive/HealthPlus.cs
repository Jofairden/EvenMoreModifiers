using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class HealthPlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower} max life");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f)
				.WithRollChance(0.333f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.statLifeMax2 += (int) Properties.RoundedPower;
		}
	}
}
