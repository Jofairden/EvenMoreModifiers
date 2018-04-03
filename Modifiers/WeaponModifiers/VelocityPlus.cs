using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class VelocityPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"+{RoundedPower}% velocity", Color = Color.Lime}
			};

		public override float MinMagnitude => 1f;
		public override float MaxMagnitude => 20f;

		public override bool CanRoll(ModifierContext ctx) 
			=> base.CanRoll(ctx) && ctx.Item.shoot > 0 && ctx.Item.shootSpeed > 0;

		public override void Apply(Item item)
		{
			base.Apply(item);
			item.shootSpeed *= RoundedPower / 100 + 1;
		}
	}
}
