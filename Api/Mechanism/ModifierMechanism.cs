using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Strategy;
using Terraria;

namespace Loot.Api.Mechanism
{
	static class ModifierMechanism
	{
		public static List<Modifier> RollModifiers(this Item item, RollingStrategy strategy, ModifierPool pool, ModifierContext context, RollingStrategyProperties properties)
		{
			return strategy._Roll(pool, context, properties);
		}
	}
}
