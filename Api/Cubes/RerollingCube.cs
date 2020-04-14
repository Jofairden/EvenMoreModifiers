using Loot.Api.Strategy;
using Terraria;

namespace Loot.Api.Cubes
{
	/// <summary>
	/// Defines a rerolling cube that opens the rerolling UI on right click
	/// The method <see cref="M:GetRollingStrategy"/> can be overridden to provide
	/// custom roll properties
	/// </summary>
	public abstract class RerollingCube : MagicalCube
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(CubeName);
			Tooltip.SetDefault(@"Can reroll the magical properties of an item
There only seems to be use for this item at a specific workbench");
			SafeStaticDefaults();
		}

		public abstract RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties);
	}
}
