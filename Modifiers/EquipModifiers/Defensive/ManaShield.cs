using System;
using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class ManaShieldEffect : ModifierEffect
	{
		public float ManaShield; // % of damage redirected to mana

		public override void ResetEffects()
		{
			ManaShield = 0f;
		}

		[AutoDelegation("OnPreHurt")]
		[DelegationPrioritization(DelegationPrioritization.Late, 100)]
		private bool ManaBlock(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			// If we have a mana shield (% damage redirected to mana)
			// Then try to redirect the damage
			int manaBlock = (int) Math.Ceiling(damage * ManaShield) * 2;
			if (manaBlock > 0 && player.statMana > 0)
			{
				// We cannot block more than how much mana we have
				if (manaBlock > player.statMana)
				{
					manaBlock = player.statMana;
				}

				damage -= manaBlock / 2;
				player.statMana -= manaBlock;
				player.manaRegenDelay = Math.Max(player.manaRegenDelay, 120);
			}

			return true;
		}
	}

	[UsesEffect(typeof(ManaShieldEffect))]
	public class ManaShield : EquipModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% of damage taken is redirected to mana");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(5f)
				.WithRollChance(0.75f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<ManaShieldEffect>().ManaShield += Properties.RoundedPower / 100f;
		}
	}
}
