using System;
using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class CritDamagePlusEffect : ModifierEffect
	{
		public float Multiplier;

		public override void OnInitialize()
		{
			Multiplier = 1f;
		}

		public override void ResetEffects()
		{
			Multiplier = 1f;
		}

		[AutoDelegation("OnModifyHitNPC")]
		[DelegationPrioritization(DelegationPrioritization.Late, 999)]
		public void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (crit)
			{
				CritBonus(ref damage);
			}
		}

		[AutoDelegation("OnModifyHitPvp")]
		[DelegationPrioritization(DelegationPrioritization.Late, 999)]
		private void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			if (crit)
			{
				CritBonus(ref damage);
			}
		}

		private void CritBonus(ref int damage)
		{
			damage = (int) Math.Ceiling(damage * Multiplier);
		}
	}

	[UsesEffect(typeof(CritDamagePlusEffect))]
	public class CritDamagePlus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% crit multiplier");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMinMagnitude(5f)
				.WithMaxMagnitude(15f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<CritDamagePlusEffect>().Multiplier += Properties.RoundedPower / 100f;
		}
	}
}
