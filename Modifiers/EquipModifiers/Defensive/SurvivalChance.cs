using System;
using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class SurvivalEffect : ModifierEffect
	{
		public float SurvivalChance; // Chance to survive lethal blow
		public static readonly float MAX_SURVIVAL_CHANCE = 0.5f;

		public override void ResetEffects()
		{
			SurvivalChance = 0f;
		}

		[AutoDelegation("OnPreKill")]
		[DelegationPrioritization(DelegationPrioritization.Late, 900)]
		private bool SurviveEvent(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Main.rand.NextFloat() < Math.Min(SurvivalChance, MAX_SURVIVAL_CHANCE))
			{
				player.statLife = 1;
				return false;
			}

			return true;
		}
	}

	[UsesEffect(typeof(SurvivalEffect))]
	public class SurvivalChance : EquipModifier
	{
		// TODO easier tooltip templating
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% chance to survive lethal blows"
				              + $"{(Main.LocalPlayer.GetModPlayer<ModifierDelegatorPlayer>().GetEffect<SurvivalEffect>().SurvivalChance >= SurvivalEffect.MAX_SURVIVAL_CHANCE ? $" (cap reached: {SurvivalEffect.MAX_SURVIVAL_CHANCE * 100f}%)" : "")}"
				);
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f)
				.WithRollChance(0.333f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<SurvivalEffect>().SurvivalChance += Properties.RoundedPower / 100f;
		}
	}
}
