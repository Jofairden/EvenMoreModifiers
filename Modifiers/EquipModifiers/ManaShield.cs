using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Modifiers.EquipModifiers
{
	public class ManaShieldEffect : ModifierEffect
	{
		public float ManaShield; // % of damage redirected to mana

		// todo needs to go after ninja dodge, but only if it fails
		
		[AutoDelegation("OnPreHurt")]
		private bool ManaBlock(ModifierPlayer player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			// If we have a mana shield (% damage redirected to mana)
			// Then try to redirect the damage
			int manaBlock = (int) Math.Ceiling(damage * ManaShield) * 2;
			if (manaBlock > 0 && player.player.statMana > 0)
			{
				// We cannot block more than how much mana we have
				if (manaBlock > player.player.statMana)
					manaBlock = player.player.statMana;

				damage -= manaBlock / 2;
				player.player.statMana -= manaBlock;
				player.player.manaRegenDelay = Math.Max(player.player.manaRegenDelay, 120);
			}

			return true;
		}
	}

	[UsesEffect(typeof(ManaShieldEffect))]
	public class ManaShield : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {Text = $"+{Properties.RoundedPower}% of damage taken is redirected to mana", Color = Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 6f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).GetEffect<ManaShieldEffect>().ManaShield += Properties.RoundedPower / 100f;
		}
	}
}