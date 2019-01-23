using Loot.Core.System.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Utility
{
	public class MiningSpeedPlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% mining speed");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(7.5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.pickSpeed -= (player.pickSpeed * Properties.RoundedPower / 100);
		}
	}
}
