using Loot.Api.Cubes;
using Loot.UI.Common;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Tabs.CraftingTab;
using Terraria;

namespace Loot.UI.Tabs.Cubing
{
	/// <summary>
	/// The tab allows using a magical cube on the slotted item to modify its potential
	/// </summary>
	internal class GuiCubingTab : CraftingTab<MagicalCube>
	{
		public override string Header => "Cubing";

		internal override CraftingComponentButton GetComponentButton()
		{
			return new GuiCubeButton(
				GuiButton.ButtonType.StoneOuterBevel,
				hintTexture: Assets.Textures.GUI.MagicalCubeTexture,
				hintText: "Place a cube here"
			);
		}

		public override bool AcceptsItem(Item item)
		{
			return ItemButton.CanTakeItem(item);
		}
	}
}
