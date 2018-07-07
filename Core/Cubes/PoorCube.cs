using Terraria;

namespace Loot.Core.Cubes
{
	public class PoorCube : MagicalCube
	{
		protected override string CubeName => "Poor Cube";
		protected override bool DisplayTier => false;

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(copper:30);
		}

		protected override void SafeStaticDefaults()
		{
		}
	}
}