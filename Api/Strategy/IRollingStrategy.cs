using Loot.Api.Core;

namespace Loot.Api.Strategy
{
	/// <summary>
	/// A strategy for rolling new modifiers on an item
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRollingStrategy<in T> where T : RollingStrategyContext
	{
		bool Roll(ModifierContext modifierContext, T strategyContext = null);
	}
}
