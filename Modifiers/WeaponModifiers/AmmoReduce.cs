using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class AmmoReduce : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"{RoundedPower}% chance to not consume ammo", Color = Color.Lime}
			};

		public override float MinMagnitude => 1f;
		public override float MaxMagnitude => 20f;

		public override bool CanRoll(ModifierContext ctx)
		{
			// Only apply on items that consume ammo
			return base.CanRoll(ctx) && ctx.Item.useAmmo > 0;
		}

		public override bool ConsumeAmmo(Item item, Player player)
		{
			return Main.rand.NextFloat() > RoundedPower / 100;
		}
	}
}
