using Loot.Core.System.Modifier;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class ThrownCritPlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% thrown crit chance");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
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
