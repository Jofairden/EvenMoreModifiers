using System;
using Loot.Core.System.Modifier;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class VelocityDamage : WeaponModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"Added damage based on player's velocity (multiplier: {Math.Round(Properties.RoundedPower / 2, 1)}x)");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(2)
				.WithRoundPrecision(1);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			float magnitude = Properties.RoundedPower * player.velocity.Length() / 4;
			damage = (int) (damage * (1 + magnitude / 100));
		}
	}
}
