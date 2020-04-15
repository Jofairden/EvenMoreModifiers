using Loot.Essences;
using Loot.UI.Common;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Tabs.CraftingTab;
using Terraria;

namespace Loot.UI.Tabs.EssenceCrafting
{
	internal class GuiEssenceTab : CraftingTab<EssenceItem>
	{
		public override string Header => "Essencecrafting";

		internal override CraftingComponentButton GetComponentButton()
		{
			return new GuiEssenceButton(
				GuiButton.ButtonType.StoneOuterBevel,
				hintTexture: Assets.Textures.PlaceholderTexture,
				hintText: "Place an essence here"
			);
		}

		public override bool AcceptsItem(Item item)
		{
			return item.modItem is EssenceItem;
		}
	}
}
