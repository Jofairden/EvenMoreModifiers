using Loot.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Utility
{
	public class WingTimePlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Math.Round(Properties.RoundedPower/60, 2)}s flight time", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 60f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.wingTimeMax += (int)Properties.RoundedPower;
		}
	}
}
