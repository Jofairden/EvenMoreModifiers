using Loot.Ext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Core
{
	public class UICubeSelectorPanel : UIInteractableItemPanel
	{
		public UICubeSelectorPanel(int netID = 0, int stack = 1, Texture2D hintTexture = null, string hintText = null) :
			base(netID, stack, hintTexture, hintText)
		{
			RightClickFunctionalityEnabled = false;
			TakeUserItemOnClick = false;
			PerformRegularClickInteraction = false;
			DrawBackgroundPanel = false;
			ShowOnlyHintOnHover = true;
			HintOnHover = "Click this cube to use it for rerolling";
			DrawStack = true;

			OnMouseOver += delegate { DrawColor = Color.White; };
			OnMouseOut += delegate
			{
				if (DrawScale != 1f)
				{
					DrawColor = Color.LightGray * 0.5f;
				}
			};
		}

		public override bool CanTakeItem(Item item)
			=> false;

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!item.IsAir && Loot.Instance.CubeRerollUI._cubePanel.CanTakeItem(item))
			{
				Loot.Instance.CubeRerollUI.MouseUp(evt);
				MouseUp(evt);
				Loot.Instance.CubeRerollUI._cubePanel.ChangeItem(item.type);
				Loot.Instance.CubeRerollUI.DetermineAvailableCubes();
			}
		}

		public void RecalculateStack()
		{
			item.stack = Main.LocalPlayer.inventory.CountItemStack(item.type, true);
			item.stack = (int)MathHelper.Clamp(item.stack, 0f, 999f);
		}
	}
}
