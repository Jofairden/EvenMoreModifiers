using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CritPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% crit chance", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 10f + 5 * (item.rare + 1));
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			return base.CanRoll(ctx) && !ctx.Item.summon;
		}

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			crit = (int)Math.Min(100, crit + Properties.RoundedPower);
		}
	}
}
