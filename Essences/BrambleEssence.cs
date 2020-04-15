using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Modifiers.EquipModifiers.Defensive;
using Terraria;

namespace Loot.Essences
{
	internal class BrambleEssence : EssenceItem
	{
		public override EssenceTier Tier => EssenceTier.I;

		public override string Description => "Grants a power that hurts enemies if they hurt you";

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.PresetLines = () => new List<Modifier>
			{
				Loot.Instance.GetModifier<Thorns>()
			};
			return base.GetRollingStrategy(item, properties);
		}
	}
}
