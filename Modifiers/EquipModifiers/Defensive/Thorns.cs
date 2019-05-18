using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class Thorns : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% thorns");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.thorns += Properties.Power / 100f;
		}
	}
}
