using System;
using Loot.Api.Attributes;
using Loot.Api.Delegators;
using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class HealthyFoesEffect : ModifierEffect
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
		[DelegationPrioritization(DelegationPrioritization.Late, 998)]
		public void ModifyHitNPC(ModifierDelegatorPlayer delegatorPlayer, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (target.life == target.lifeMax)
			{
				HealthyFoes(ref damage);
			}
		}

		[AutoDelegation("OnModifyHitPvp")]
		[DelegationPrioritization(DelegationPrioritization.Late, 998)]
		private void ModifyHitPvp(ModifierDelegatorPlayer delegatorPlayer, Item item, Player target, ref int damage, ref bool crit)
		{
			if (target.statLife == target.statLifeMax2)
			{
				HealthyFoes(ref damage);
			}
		}

		private void HealthyFoes(ref int damage)
		{
			damage = (int) (Math.Ceiling(damage * Multiplier));
		}
	}

	[UsesEffect(typeof(HealthyFoesEffect))]
	public class HealthyFoesBonus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% damage vs max life foes");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMinMagnitude(5f)
				.WithMaxMagnitude(15f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<HealthyFoesEffect>().Multiplier += Properties.RoundedPower / 100f;
		}
	}
}
