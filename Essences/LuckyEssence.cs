using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Modifiers.EquipModifiers.Utility;
using Terraria;

namespace Loot.Essences
{
	internal class LuckyEssence : EssenceItem
	{
		public override EssenceTier Tier => EssenceTier.I;

		public override string Description => "Could it possibly improve your luck?";

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.PresetLines = () => new List<Modifier>
			{
				Loot.Instance.GetModifier<LuckPlus>()
			};
			return base.GetRollingStrategy(item, properties);
		}
	}
}
