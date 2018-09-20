using Loot.Core.Cubes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace Loot.UI
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

		public override bool CanTakeItem(Item item) => item.modItem is MagicalCube;

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!this.item.IsAir)
			{
				RecalculateStack();
			}
		}

		public void InteractionLogic(ItemRollProperties itemRollProperties)
		{
			(item.modItem as MagicalCube)?.SetRollLogic(itemRollProperties);
		}

		internal void RecalculateStack()
		{
			// @todo track cube tier as well
			// after cube is slotted, count total number

			// .Take 58 because 59th slot is MouseItem for some reason.
			int stack = Main.LocalPlayer.inventory.Take(58)
				.Where(x => x.type == item.type)
				.Select(x => x.stack)
				.Sum();

			if (Main.mouseItem?.type == item.type)
			{
				stack += Main.mouseItem.stack;
			}

			stack = (int)MathHelper.Clamp(stack, 0f, 999f);

			this.item.stack = stack;
		}
	}
}
