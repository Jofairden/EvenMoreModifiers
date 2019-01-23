using Loot.Core.System.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Utility
{
	public class FishingPlus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower} fishing skill [WIP]");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(2f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.fishingSkill += (int) Properties.RoundedPower;
		}
	}
}
