using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Modifiers.WeaponModifiers;
using Terraria;

namespace Loot.Essences
{
	internal class BloodEssence : EssenceItem
	{
		public override EssenceTier Tier => EssenceTier.I;

		public override string Description => "Grants a more power based on missing health";

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.PresetLines = () => new List<Modifier>
			{
				Loot.Instance.GetModifier<MissingHealthDamage>()
			};
			return base.GetRollingStrategy(item, properties);
		}
	}
}
