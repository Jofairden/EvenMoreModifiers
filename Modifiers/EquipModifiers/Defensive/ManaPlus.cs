using Loot.Core;
using Loot.Core.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class ManaPlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower} max mana", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 5f, rollChance: 0.333f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.statManaMax2 += (int)Properties.RoundedPower;
		}
	}
}
