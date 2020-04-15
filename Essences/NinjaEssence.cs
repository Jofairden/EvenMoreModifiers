using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Modifiers.EquipModifiers.Defensive;
using Loot.Modifiers.WeaponModifiers;
using Terraria;

namespace Loot.Essences
{
	internal class NinjaEssence : EssenceItem
	{
		public override EssenceTier Tier => EssenceTier.I;

		public override string Description => "“If you're trying to disguise yourself to fool an approaching enemy, then try impersonating a nutsack. Just puff out your cheeks and look uninterested.”";

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.PresetLines = () => new List<Modifier>
			{
				Loot.Instance.GetModifier<DodgeChance>()
			};
			return RollingUtils.Strategies.Default;
		}
	}
}
