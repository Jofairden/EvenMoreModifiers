using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DamagePlusDayEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage during the day", Color = Color.Lime}
			};

		public override float MinMagnitude => 1.0f / 15;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 15f;

		public override void UpdateItem(ModifierContext ctx, bool isItemEquipped = false)
		{
			ModifierItem.GetItemInfo(ctx.Item).dayDamageBonus = Power;
		}
	}
}
