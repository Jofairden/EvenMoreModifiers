using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Api.Core;
using Loot.Api.Strategy;
using Loot.RollingStrategies;
using Terraria;

namespace Loot.Api.Mechanism
{
	static class ModifierMechanism
	{
		public static List<Modifier> RollModifiers(Item item, ModifierPool pool, ModifierContext context, RollingStrategyProperties properties)
		{
			return new NormalRollingStrategy().Roll(
				pool,
				context,
				new RollingStrategyContext(item, properties)
			);
		}
	}
}
