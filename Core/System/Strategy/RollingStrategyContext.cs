using Terraria;

namespace Loot.Core.System.Strategy
{
	/// <summary>
	/// The context for a <see cref="IRollingStrategy{T}"/>
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
