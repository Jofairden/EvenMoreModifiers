using Loot.Ext;
using Loot.UI.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Sealing
{
	public class UISealItemPanel : UIInteractableItemPanel
	{
		public UISealItemPanel(int netID = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null) :
			base(netID, stack, hintTexture, hintText)
		{
			this.RightClickFunctionalityEnabled = false;
			this.HintOnHover = " (click to take item)";
		}

		public override bool CanTakeItem(Item givenItem) => base.CanTakeItem(givenItem) && givenItem.IsModifierRollableItem();

		public override void PreOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!item.IsAir)
			{
				EMMItem.GetItemInfo(item).SlottedInCubeUI = false;
			}
		}

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!item.IsAir)
			{
				EMMItem.GetItemInfo(item).SlottedInCubeUI = true;
			}
		}
	}
}
