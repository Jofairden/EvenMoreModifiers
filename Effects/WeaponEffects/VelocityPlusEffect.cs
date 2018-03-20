using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class VelocityPlusEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% velocity", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.05f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 20f;

		public override bool CanRoll(ModifierContext ctx) => ctx.Item.shoot > 0 && ctx.Item.shootSpeed > 0;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.shootSpeed *= Power / 100 + 1;
		}
	}
}
