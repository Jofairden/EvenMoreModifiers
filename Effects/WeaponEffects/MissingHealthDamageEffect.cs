using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class MissingHealthDamageEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"Up to +{(int)Math.Round(Power*6)}% damage based on missing health", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.2f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 5f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			ModifierItem.GetItemInfo(ctx.Item).missingHealthBonus = Power;
		}
	}
}
