using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Modifiers.EquipModifiers.Defensive;
using Terraria;

namespace Loot.Essences
{
	internal class GodlyEssence : EssenceItem
	{
		public override EssenceTier Tier => EssenceTier.I;

		public override string Description => "Grants a power that makes you more godlike";

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.PresetLines = () => new List<Modifier>
			{
				Loot.Instance.GetModifier<ImmunityTimePlus>()
			};
			return base.GetRollingStrategy(item, properties);
		}
	}
}
