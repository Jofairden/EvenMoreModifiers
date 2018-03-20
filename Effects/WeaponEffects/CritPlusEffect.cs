using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class CritPlusEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% crit chance", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.crit += (int)Math.Round(Power);
		}
	}
}
