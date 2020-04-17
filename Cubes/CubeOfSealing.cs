using Loot.Api.Cubes;
using Loot.Api.Strategy;
using Loot.RollingStrategies;
using Terraria;

namespace Loot.Cubes
{
	/// <summary>
	/// A cube of sealing is used to lock modifiers in place on an item
	/// </summary>
	public class CubeOfSealing : RerollingCube
	{
		protected override int EssenceCraftCost => 10;
		protected override string CubeName => "Sealing Cube";

		protected override void SafeStaticDefaults()
		{
			Tooltip.SetDefault("Allows sealing an item's modifiers" +
							   "\nSealing modifiers means they cannot be changed");
		}

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(copper: 1);
		}

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			return new SealingRollingStrategy();
		}
	}
}
