using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class KnockbackPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% knockback", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: 4f + item.rare + 1f, maxMagnitude: 10f + 5f * (item.rare + 1));
		}

		public override bool CanRoll(ModifierContext ctx)
			=> base.CanRoll(ctx) && ctx.Item.knockBack > 0;

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			base.GetWeaponKnockback(item, player, ref knockback);
			knockback *= Properties.RoundedPower / 100f + 1;
		}
	}
}
