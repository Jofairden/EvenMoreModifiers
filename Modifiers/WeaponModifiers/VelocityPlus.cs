using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class VelocityPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% velocity", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: 5f + 5f * (item.rare + 1), maxMagnitude: 20f + 10f * (item.rare + 1));
		}

		public override bool CanRoll(ModifierContext ctx)
			=> base.CanRoll(ctx) && ctx.Item.shoot > 0 && ctx.Item.shootSpeed > 0;

		public override void Apply(Item item)
		{
			base.Apply(item);
			item.shootSpeed += (int)Math.Ceiling(item.shootSpeed * (Properties.RoundedPower / 100));
		}
	}
}
