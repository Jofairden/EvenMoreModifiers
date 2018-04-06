using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class VelocityDamage : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"Added damage based on your velocity (multiplier: {Math.Round(RoundedPower/2, 1)}x)", Color = Color.Lime}
			};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 7f;
		public override int GetRoundPrecision(Item item) => 1;

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			// Formula ported from old mod
			float magnitude = RoundedPower * player.velocity.Length() / 4;
			damage = (int)(damage * (1 + magnitude / 100));
		}
	}
}
