using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Loot.Modifiers.EquipModifiers
{
	public class KnockbackImmunity : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"Knockback immunity", Color =  Color.LimeGreen},
		};

		public override float GetRollChance(Item item) => 1/3f;   // 1/3rd the chance of other modifiers
		public override float GetRarityLevel(Item item) => 3f;    // Also worth 3 times as much as normal modifiers

		public override bool CanRoll(ModifierContext ctx)
		{
			// Don't roll on items that already provide knockback immunity
			switch(ctx.Item.type)
			{
				case (ItemID.CobaltShield):
				case (ItemID.ObsidianShield):
				case (ItemID.AnkhShield):
					return false;
			}
			return true;
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.noKnockback = true;
		}
	}
}
