using Loot.Api.Core;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Utility
{
	public class MoveSpeedPlus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% movement speed");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithBasePower(1 / 5f)
				.WithMaxMagnitude(50f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.moveSpeed += Properties.RoundedPower / 100;
			player.maxRunSpeed *= 1 + Properties.RoundedPower / 100;
		}
	}
}
