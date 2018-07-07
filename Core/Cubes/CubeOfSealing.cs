using Loot.UI;
using Terraria;

namespace Loot.Core.Cubes
{
	public class CubeOfSealing : MagicalCube
	{
		protected override string CubeName => "Cube of Sealing";
		protected override bool DisplayTier => false;

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
			if (!Loot.Instance.CubeSealUI.Visible || Loot.Instance.CubeSealUI.Visible && Loot.Instance.CubeSealUI._itemPanel.item.IsAir)
			{
				Loot.Instance.CubeSealUI.ToggleUI(Loot.Instance.CubeInterface, Loot.Instance.CubeSealUI);
			}

			item.stack++;
		}
	}
}