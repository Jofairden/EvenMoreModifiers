using System.Collections.Generic;
using Loot.Api.Core;
using Terraria;

namespace Loot.Api.Strategy
{
	public abstract class RollingStrategy<T> where T : RollingStrategyContext
	{
		public abstract List<Modifier> Roll(ModifierPool pool, ModifierContext modifierContext, T context);
	}

}
