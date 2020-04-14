using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Loaders;
using Terraria.Utilities;

namespace Loot.Api.Mechanism
{
	static class ModifierPoolMechanism
	{
		private static IEnumerable<ModifierPool> Pools => ContentLoader.ModifierPool.Pools;

		public static ModifierPool GetPool(ModifierContext context)
		{
			if (context.Pool != null) return context.Pool;
			var set = context.PoolSet ?? Pools.Where(x => x._CanRoll(context));
			return GetWeightedPool(set);
		}

		private static ModifierPool GetWeightedPool(IEnumerable<ModifierPool> set)
		{
			var wr = new WeightedRandom<ModifierPool>();
			foreach (var m in set.Where(x => x.Type > 0))
			{
				wr.Add(m, m.RollChance);
			}

			var mod = wr.Get();
			return (ModifierPool)mod?.Clone();
		}
	}
}
