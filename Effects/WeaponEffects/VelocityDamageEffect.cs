using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class VelocityDamageEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"Added damage based on velocity (multiplier: {Math.Round(Power/2, 1)}x)", Color = Color.Lime}
			};

		public override float MinMagnitude => 1f / 7;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 7f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			ModifierItem.GetItemInfo(ctx.Item).velocityDamageBonus = Power;
		}
	}
}
