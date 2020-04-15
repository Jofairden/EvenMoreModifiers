using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Modifiers.WeaponModifiers;
using Terraria;

namespace Loot.Essences
{
	internal class CursedEssence : EssenceItem
	{
		public override EssenceTier Tier => EssenceTier.I;

		public override string Description => "Grants a powerful curse...";

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.PresetLines = () => new List<Modifier>
			{
				Loot.Instance.GetModifier<CursedDamage>()
			};
			return base.GetRollingStrategy(item, properties);
		}
	}
}
