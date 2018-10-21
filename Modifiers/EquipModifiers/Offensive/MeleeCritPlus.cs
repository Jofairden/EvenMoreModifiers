using Loot.Core;
using Loot.Core.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Offensive
{
	public class MeleeCritPlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% melee crit chance", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.meleeCrit += (int)Properties.RoundedPower;
		}
	}
}
