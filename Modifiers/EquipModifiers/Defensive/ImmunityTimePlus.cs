using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class ImmunityEffect : ModifierEffect
	{
		public int BonusImmunityTime; // Extra immunity frames

		public override void ResetEffects()
		{
			BonusImmunityTime = 0;
		}

		[AutoDelegation("OnPostHurt")]
		[DelegationPrioritization(DelegationPrioritization.Late, 800)]
		private void Immunity(ModifierDelegatorPlayer delegatorPlayer, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			int frames = damage <= 1
				? BonusImmunityTime / 2
				: BonusImmunityTime;
			if (delegatorPlayer.player.immuneTime > 0)
			{
				delegatorPlayer.player.immuneTime += frames;
			}
		}
	}

	[UsesEffect(typeof(ImmunityEffect))]
	public class ImmunityTimePlus : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower} immunity frames");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(3f)
				.WithRollChance(0.222f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<ImmunityEffect>().BonusImmunityTime += (int) Properties.RoundedPower;
		}
	}
}
