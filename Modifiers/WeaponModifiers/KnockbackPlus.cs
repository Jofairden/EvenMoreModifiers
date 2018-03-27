using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class KnockbackPlus : WeaponModifier
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% knockback", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.05f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 20f;

		public override bool CanRoll(ModifierContext ctx) 
			=> base.CanRoll(ctx) && ctx.Item.knockBack > 0;

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			base.GetWeaponKnockback(item, player, ref knockback);
			knockback *= Power / 100 + 1;
		}
	}
}
