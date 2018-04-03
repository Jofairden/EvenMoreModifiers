﻿using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class MeleeCritPlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower}% melee crit chance", Color =  Color.LimeGreen},
		};

		public override float MinMagnitude => 1f;
		public override float MaxMagnitude => 8f;
		
		public override void UpdateEquip(Item item, Player player)
		{
			player.meleeCrit += (int)RoundedPower;
		}
	}
}
