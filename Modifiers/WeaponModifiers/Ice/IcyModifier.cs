using Loot.Core.System;
using Terraria;
using Terraria.ID;

namespace Loot.Modifiers.WeaponModifiers.Ice
{
	public class IcyModifier : IceModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"Inflict frostburn on hit for {Properties.RoundedPower}s");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithRollChance(0.125f)
				.WithMinMagnitude(0.5f)
				.WithMaxMagnitude(3f)
				.IsUniqueModifier(true);
		}

		public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
		{
			target.AddBuff(BuffID.Frostburn, (int) (Properties.Power * 60));
		}
	}
}
