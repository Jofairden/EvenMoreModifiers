using Loot.Ext;
using Loot.UI.Common.Controls.Button;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common.Tabs.Cubing
{
	internal class GuiCubeItemButton : GuiInteractableItemButton
	{
		internal GuiCubeItemButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			RightClickFunctionalityEnabled = false;
			HintOnHover = " (click to take item)";
		}

		public override bool CanTakeItem(Item givenItem) => base.CanTakeItem(givenItem) && givenItem.IsModifierRollableItem();
		//&& !EMMItem.GetItemInfo(givenItem).SealedModifiers // Omitted for now, cubing tab is used for sealing

		public override void PreOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!Item.IsAir)
			{
				EMMItem.GetItemInfo(Item).SlottedInCubeUI = false;
			}
		}

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!Item.IsAir)
			{
				EMMItem.GetItemInfo(Item).SlottedInCubeUI = true;
			}
		}
	}
}
