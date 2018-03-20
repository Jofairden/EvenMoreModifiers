using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class AmmoReduceEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"{(int)Math.Round(Power)}% chance to not consume ammo", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.05f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 20f;

		public override bool CanRoll(ModifierContext ctx)
		{
			// Only apply on items that consume ammo
			return ctx.Item.useAmmo > 0;
		}

		public override void HoldItem(ModifierContext ctx)
		{
			ModifierItem.GetItemInfo(ctx.Item).dontConsumeAmmo = Power / 100;
		}
	}
}
