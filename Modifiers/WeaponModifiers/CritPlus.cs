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

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			crit += RoundedPower;
		}
	}
}
