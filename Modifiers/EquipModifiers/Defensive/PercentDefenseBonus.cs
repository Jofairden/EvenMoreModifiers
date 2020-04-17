using System;
using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class PercentDefBoostEffect : ModifierEffect
	{
		public float PercentDefBoost;

		public override void ResetEffects()
		{
			PercentDefBoost = 0f;
		}

		[AutoDelegation("OnPostUpdateEquips")]
		[DelegationPrioritization(DelegationPrioritization.Late, 999)]
		private void DefBoost()
		{
			player.statDefense = (int) Math.Ceiling(player.statDefense * (1 + PercentDefBoost));
		}
	}

	[UsesEffect(typeof(PercentDefBoostEffect))]
	public class PercentDefenseBonus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% defense");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<PercentDefBoostEffect>().PercentDefBoost += Properties.RoundedPower / 100f;
		}
	}
}
