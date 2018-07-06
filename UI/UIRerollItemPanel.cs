using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI
{
	public class UIRerollItemPanel : UIInteractableItemPanel
	{
		public UIRerollItemPanel(int netID = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null) :
			base(netID, stack, hintTexture, hintText)
		{
			this.RightClickFunctionalityEnabled = false;
			this.HintOnHover = " (click to take take item)";
		}

		public override bool CanTakeItem(Item item) => item.IsModifierRollableItem();

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			Loot.Instance.cubeUI.UpdateModifierLines();
		}
	}
}