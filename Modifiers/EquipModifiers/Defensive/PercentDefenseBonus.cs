using System;
using Loot.Core.Attributes;
using Loot.Core.System;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class PercentDefBoostEffect : ModifierEffect
	{
		public float PercentDefBoost;

		public override void ResetEffects(ModifierPlayer player)
		{
			PercentDefBoost = 0f;
		}

		[AutoDelegation("OnPostUpdateEquips")]
		[DelegationPrioritization(DelegationPrioritization.Late, 999)]
		private void DefBoost(ModifierPlayer player)
		{
			player.player.statDefense = (int) Math.Ceiling(player.player.statDefense * (1 + PercentDefBoost));
		}
	}

	[UsesEffect(typeof(PercentDefBoostEffect))]
	public class PercentDefenseBonus : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% defense");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).GetEffect<PercentDefBoostEffect>().PercentDefBoost += Properties.RoundedPower / 100f;
		}
	}
}
