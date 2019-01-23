using Loot.Core.System;
using Terraria;
using Terraria.ID;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class KnockbackImmunity : EquipModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"Knockback immunity");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithRollChance(0.333f)
				.WithRarityLevel(3f)
				.IsUniqueModifier(true);
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			// Don't roll on items that already provide knockback immunity
			switch (ctx.Item.type)
			{
				default:
					return base.CanRoll(ctx);
				case (ItemID.CobaltShield):
				case (ItemID.ObsidianShield):
				case (ItemID.AnkhShield):
					return false;
			}
		}

		public override void UpdateEquip(Item item, Player player)
		{
			player.noKnockback = true;
		}
	}
}
