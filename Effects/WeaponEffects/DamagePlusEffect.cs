using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;

namespace Loot.Effects.WeaponEffects
{
	public class DamagePlusEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.damage = (int)Math.Ceiling(ctx.Item.damage * (1 + Power / 100f));
		}
	}
}
