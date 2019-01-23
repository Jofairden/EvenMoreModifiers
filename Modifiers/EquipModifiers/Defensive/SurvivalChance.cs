using System;
using Loot.Core.Attributes;
using Loot.Core.System.Modifier;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class SurvivalEffect : ModifierEffect
	{
		public float SurvivalChance; // Chance to survive lethal blow
		public static readonly float MAX_SURVIVAL_CHANCE = 0.5f;

		public override void ResetEffects(ModifierPlayer player)
		{
			SurvivalChance = 0f;
		}

		[AutoDelegation("OnPreKill")]
		[DelegationPrioritization(DelegationPrioritization.Late, 900)]
		private bool SurviveEvent(ModifierPlayer player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Main.rand.NextFloat() < Math.Min(SurvivalChance, MAX_SURVIVAL_CHANCE))
			{
				player.player.statLife = 1;
				return false;
			}

			return true;
		}
	}

	[UsesEffect(typeof(SurvivalEffect))]
	public class SurvivalChance : EquipModifier
	{
		// TODO easier tooltip templating
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% chance to survive lethal blows"
				              + $"{(Main.LocalPlayer.GetModPlayer<ModifierPlayer>().GetEffect<SurvivalEffect>().SurvivalChance >= SurvivalEffect.MAX_SURVIVAL_CHANCE ? $" (cap reached: {SurvivalEffect.MAX_SURVIVAL_CHANCE * 100f}%)" : "")}"
				);
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f)
				.WithRollChance(0.333f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).GetEffect<SurvivalEffect>().SurvivalChance += Properties.RoundedPower / 100f;
		}
	}
}
