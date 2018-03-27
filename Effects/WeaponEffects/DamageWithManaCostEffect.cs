using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DamageWithManaCostEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage, but added mana cost", Color = Color.Lime}
			};

		public override float MinMagnitude => 16f / 30;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 30f;

		public override bool CanRoll(ModifierContext ctx) => ctx.Item.mana == 0;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.damage = (int)Math.Ceiling(ctx.Item.damage * (1 + Power / 100f));
			ctx.Item.mana = Math.Max((int)(25 * (ctx.Item.useTime / 60.0)), 1);
		}
	}
}
