using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class SpeedPlus : WeaponModifier
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% speed", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void Apply(Item item)
		{
			base.Apply(item);
			// Floor the effect so that it will always increase speed by at least 1 frame
			item.useTime = (int)Math.Floor((float)item.useTime * (1 - Power / 100f));
			item.useAnimation = (int)Math.Floor((float)item.useAnimation * (1 - Power / 100f));

			// Don't go below the minimum
			if (item.useTime < 2) item.useTime = 2;
			if (item.useAnimation < 2) item.useAnimation = 2;
		}
	}
}
