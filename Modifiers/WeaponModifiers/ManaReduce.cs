using Microsoft.Xna.Framework;
using System;
using Loot.Core.System;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class ManaReduce : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"-{Properties.RoundedPower}% mana cost", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 10f);
		}

		public override bool CanRoll(ModifierContext ctx)
			=> base.CanRoll(ctx) && ctx.Item.mana > 0;

		public override void Apply(Item item)
		{
			base.Apply(item);
			// Always reduce by at least 1 mana cost
			item.mana = (int)Math.Floor(item.mana * (1 - Properties.RoundedPower / 100f));

			// Don't go below 1 mana cost! 0 cost is too OP :P
			if (item.mana < 1)
			{
				item.mana = 1;
			}
		}
	}
}
