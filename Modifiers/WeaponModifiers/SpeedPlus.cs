using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class SpeedPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"+{RoundedPower}% speed", Color = Color.Lime}
			};

		public override float MinMagnitude => 1f;
		public override float MaxMagnitude => 10f;

		public override void Apply(Item item)
		{
			base.Apply(item);
			// Floor the effect so that it will always increase speed by at least 1 frame
			item.useTime = (int)Math.Floor((float)item.useTime * (1 - RoundedPower / 100f));
			item.useAnimation = (int)Math.Floor((float)item.useAnimation * (1 - RoundedPower / 100f));

			// Don't go below the minimum
			if (item.useTime < 2) item.useTime = 2;
			if (item.useAnimation < 2) item.useAnimation = 2;
		}
	}
}
