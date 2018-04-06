﻿using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class MissingHealthDamage : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"Up to +{RoundedPower}% damage based on missing health", Color = Color.Lime}
			};

		public override float GetMinMagnitude(Item item) => 6f;
		public override float GetMaxMagnitude(Item item) => 30f;

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			// Formula ported from old mod
			float mag = (RoundedPower * ((player.statLifeMax2 - player.statLife) / (float)player.statLifeMax2));
			damage = (int)(damage * (1 + mag / 100));
		}
	}
}
