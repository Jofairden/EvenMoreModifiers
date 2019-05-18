using Loot.Api.Strategy;
using Loot.RollingStrategies;

namespace Loot.Api.Ext
{
	public static class RollingUtils
	{
		public static class Properties
		{
			public static RollingStrategyProperties WorldGen => new RollingStrategyProperties
			{
				MaxRollableLines = 2,
				ExtraLuck = 0,
				CanUpgradeRarity = context => false
			};
		}

		public static class Strategies
		{
			public static NormalRollingStrategy Normal => new NormalRollingStrategy();
		}
	}
}
