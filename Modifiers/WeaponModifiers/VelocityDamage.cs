using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class VelocityDamage : WeaponModifier
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"Added damage based on velocity (multiplier: {Math.Round(Power/2, 1)}x)", Color = Color.Lime}
			};

		public override float MinMagnitude => 1f / 7;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 7f;

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			// Formula ported from old mod
			float magnitude = Power * player.velocity.Length() / 4;
			damage = (int)(damage * (1 + magnitude / 100));
		}
	}
}
