﻿using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class ImmunityTimePlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower} immunity frames", Color =  Color.LimeGreen},
		};

		public override float MinMagnitude => 1f;
		public override float MaxMagnitude => 20f;

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.PlayerInfo(player).BonusImmunityTime += (int)RoundedPower;
		}
	}
}
