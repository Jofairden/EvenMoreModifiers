using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CritPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"+{RoundedPower}% crit chance", Color = Color.Lime}
			};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 10f;

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			crit += (int)RoundedPower;
		}
	}
}
