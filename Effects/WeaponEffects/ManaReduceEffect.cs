using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class ManaReduceEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"-{(int)Math.Round(Power)}% mana cost", Color = Color.Lime}
			};

		public override float MinMagnitude => (float)1 / 15;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 15f;

		public override bool CanRoll(ModifierContext ctx) => ctx.Item.mana > 0;

		public override void ApplyItem(ModifierContext ctx)
		{
			// Always reduce by at least 1 mana cost
			ctx.Item.mana = (int)Math.Floor(ctx.Item.mana * (1 - Power / 100f));

			// Don't go below 1 mana cost! 0 cost is too OP :P
			if (ctx.Item.mana < 1) ctx.Item.mana = 1;
		}
	}
}
