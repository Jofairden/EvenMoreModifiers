using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class ThrownCritPlus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% thrown crit chance");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.thrownCrit += (int) Properties.RoundedPower;
		}
	}
}
