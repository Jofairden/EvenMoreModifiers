using System;
using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class VelocityDamage : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"Added damage based on player's velocity (multiplier: {Math.Round(Properties.RoundedPower / 2, 1)}x)");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(2)
				.WithRoundPrecision(1);
		}

		public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
		{
			base.ModifyWeaponDamage(item, player, ref add, ref mult, ref flat);
			float magnitude = Properties.RoundedPower * player.velocity.Length() / 4;
			add += 1 + magnitude / 100;
		}
	}
}
