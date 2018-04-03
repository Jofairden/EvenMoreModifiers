using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class KnockbackImmunity : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"Knockback immunity", Color =  Color.LimeGreen},
		};

		public override float RollChance => 1/3f;   // 1/3rd the chance of other modifiers
		public override float RarityLevel => 3f;    // Also worth 3 times as much as normal modifiers

		public override void UpdateEquip(Item item, Player player)
		{
			player.noKnockback = true;
		}
	}
}
