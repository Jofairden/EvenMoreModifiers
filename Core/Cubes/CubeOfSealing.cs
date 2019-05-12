using Loot.Sounds;
using Loot.UI.Common.Tabs.Cubing;
using Terraria;

namespace Loot.Core.Cubes
{
	/// <summary>
	/// A cube of sealing is used to lock modifiers in place on an item
	/// </summary>
	public class CubeOfSealing : RerollingCube
	{
		protected override string CubeName => "Sealing Cube";

		protected override void SafeStaticDefaults()
		{
			Tooltip.SetDefault("Press left control and right click to open cube UI" +
							   "\nAllows sealing an item's modifiers" +
							   "\nSealing modifiers means they cannot be changed" +
							   "\nCube is consumed upon use");
		}

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(silver: 50);
		}

		public override void RightClick(Player player)
		{
			// TODO for now, just use cubing UI
			base.RightClick(player);
		}
	}
}
