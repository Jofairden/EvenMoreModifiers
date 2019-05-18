using Loot.Api.Attributes;
using Loot.Api.Delegators;
using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class LifeRegenEffect : ModifierEffect
	{
		public int LifeRegen;

		public override void ResetEffects()
		{
			LifeRegen = 0;
		}

		[AutoDelegation("OnUpdateLifeRegen")]
		private void Regen(ModifierDelegatorPlayer delegatorPlayer)
		{
			delegatorPlayer.player.lifeRegen += LifeRegen / 30;
			LifeRegen %= 30;
		}
	}

	[UsesEffect(typeof(LifeRegenEffect))]
	public class LifeRegen : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower} life regen/minute");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(15f)
				.WithRollChance(0.5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<LifeRegenEffect>().LifeRegen += (int) Properties.RoundedPower;
		}
	}
}
