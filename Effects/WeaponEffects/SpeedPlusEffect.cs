using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class SpeedPlusEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% speed", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void ApplyItem(ModifierContext ctx)
		{
			// Floor the effect so that it will always increase speed by at least 1 frame
			ctx.Item.useTime = (int)Math.Floor((float)ctx.Item.useTime * (1 - Power / 100f));
			ctx.Item.useAnimation = (int)Math.Floor((float)ctx.Item.useAnimation * (1 - Power / 100f));

			// Don't go below the minimum
			if (ctx.Item.useTime < 2) ctx.Item.useTime = 2;
			if (ctx.Item.useAnimation < 2) ctx.Item.useAnimation = 2;
		}
	}
}
