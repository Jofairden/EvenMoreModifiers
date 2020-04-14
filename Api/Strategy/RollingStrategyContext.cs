using Terraria;

namespace Loot.Api.Strategy
{
	/// <summary>
	/// The context in which a rolling strategy is executed
	/// </summary>
	public class RollingStrategyContext
	{
		public readonly Item Item;
		public RollingStrategyProperties Properties;

		public RollingStrategyContext(Item item, RollingStrategyProperties properties)
		{
			Item = item;
			Properties = properties;
		}
	}
}
