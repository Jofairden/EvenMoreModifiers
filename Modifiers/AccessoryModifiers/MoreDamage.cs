using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.AccessoryModifiers
{
	public class MoreDamage : AccessoryModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = "Player deals 100% more damage", Color =  Color.SlateGray},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(rollChance: 0f, rarityLevel: 5f);
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			player.magicDamage *= 2f;
			player.meleeDamage *= 2f;
			player.rangedDamage *= 2f;
			player.minionDamage *= 2f;
		}
	}
}
