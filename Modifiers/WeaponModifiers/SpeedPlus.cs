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

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 10f;

		public override void Apply(Item item)
		{
			base.Apply(item);
			
			item.useTime = (int)(item.useTime * (1 - RoundedPower / 100f));
			item.useAnimation = (int)(item.useAnimation * (1 - RoundedPower / 100f));

			// Don't go below the minimum
			if (item.useTime < 2) item.useTime = 2;
			if (item.useAnimation < 2) item.useAnimation = 2;
		}

		/*public override float UseTimeMultiplier(Item item, Player player)
		{
			return 1 - RoundedPower / 100;
		}*/
	}
}
