using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class ManaReduce : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"-{RoundedPower}% mana cost", Color = Color.Lime}
			};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 15f;

		public override bool CanRoll(ModifierContext ctx) 
			=> base.CanRoll(ctx) && ctx.Item.mana > 0;

		public override void Apply(Item item)
		{
			base.Apply(item);
			// Always reduce by at least 1 mana cost
			item.mana = (int)Math.Floor(item.mana * (1 - RoundedPower / 100f));

			// Don't go below 1 mana cost! 0 cost is too OP :P
			if (item.mana < 1)
				item.mana = 1;
		}
	}
}
