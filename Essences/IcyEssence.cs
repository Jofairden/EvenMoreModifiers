using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Modifiers.WeaponModifiers.Ice;
using Terraria;

namespace Loot.Essences
{


	sealed class IcyEssence : EssenceItem
	{
		public override EssenceTier Tier => EssenceTier.I;

		public override string Description => "Grants the \"Icy\" modifier";

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.PresetLines = () => new List<Modifier>
			{
				Loot.Instance.GetModifier<IcyModifier>()
			};
			properties.MaxRollableLines = 2;
			return RollingUtils.Strategies.Default;
		}
	}
}
