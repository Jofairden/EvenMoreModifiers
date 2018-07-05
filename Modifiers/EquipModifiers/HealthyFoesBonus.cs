using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class HealthyFoesBonus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {Text = $"+{Properties.RoundedPower}% damage vs max life foes", Color = Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 20f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).HealthyFoesMulti += Properties.RoundedPower / 100f;
		}

		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			ModifierPlayer.Player(player).HealthyFoesMulti -= Properties.RoundedPower / 100f;
		}

		[AutoDelegation("OnModifyHitNPC")]
		private void HealthyBonusNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (target.life == target.lifeMax)
				damage = (int) (Math.Ceiling(damage * ModifierPlayer.Player(player).HealthyFoesMulti));
		}

		[AutoDelegation("OnModifyHitPvp")]
		private void HealthyBonusPvp(Player player, Item item, Player target, ref int damage, ref bool crit)
		{
			if (target.statLife == target.statLifeMax2)
				damage = (int) (Math.Ceiling(damage * ModifierPlayer.Player(player).HealthyFoesMulti));
		}
	}
}