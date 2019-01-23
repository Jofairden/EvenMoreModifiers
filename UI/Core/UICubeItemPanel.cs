using Loot.Core.Cubes;
using Loot.Ext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Core
{
	public class UICubeItemPanel : UIInteractableItemPanel
	{
		public UICubeItemPanel(int netID = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null) :
			base(netID, stack, hintTexture, hintText)
		{
			this.RightClickFunctionalityEnabled = false;
			this.TakeUserItemOnClick = false;
			this.HintOnHover = " (click to unslot)";
		}

		public override bool CanTakeItem(Item item) => item.modItem is RerollingCube;

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!this.item.IsAir)
			{
				RecalculateStack();
			}

			Loot.Instance.CubeRerollUI.DetermineAvailableCubes();
		}

		public void ChangeItem(int type)
		{
			item.SetDefaults(type);
			RecalculateStack();
		}

		public void InteractionLogic(ItemRollProperties itemRollProperties)
		{
			(item.modItem as RerollingCube)?.SetRollLogic(itemRollProperties);
		}

		public void RecalculateStack()
		{
			item.stack = Main.LocalPlayer.inventory.CountItemStack(item.type, true);
			item.stack = (int) MathHelper.Clamp(item.stack, 0f, 999f);
		}
	}
}
