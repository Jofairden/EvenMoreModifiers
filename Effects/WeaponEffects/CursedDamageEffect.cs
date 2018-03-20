using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class CursedDamageEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage, but cursed", Color = Color.Lime}
			};

		public override float MinMagnitude => 16f / 30;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 30f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.damage = (int)Math.Ceiling(ctx.Item.damage * (1 + Power / 100f));
		}

		public override void HoldItem(ModifierContext ctx)
		{
			ModifierPlayer.PlayerInfo(ctx.Player).holdingCursed = true;
		}
	}
}
