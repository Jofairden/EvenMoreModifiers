using Terraria;

namespace Loot.Core.Cubes
{
	public class BasicCube : MagicalCube
	{
		protected override string CubeName => "Basic Cube";

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(copper:30);
		}

		protected override void SafeStaticDefaults()
		{
		}
	}
}