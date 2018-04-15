using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class ThrownCritPlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% thrown crit chance", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 8f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.thrownCrit += (int)Properties.RoundedPower;
		}
	}
}
