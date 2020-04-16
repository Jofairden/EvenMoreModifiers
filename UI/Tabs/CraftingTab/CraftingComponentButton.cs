using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.UI.Common.Controls.Button;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Tabs.CraftingTab
{
	/**
	 * Defines a component button used for crafting
	 * Is an interactable item button with some additional behavior
	 */
	internal abstract class CraftingComponentButton : GuiInteractableItemButton
	{
		internal CraftingComponentButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			RightClickFunctionalityEnabled = false;
			TakeUserItemOnClick = false;
			HintOnHover = " (click to unslot)";
		}

		public abstract RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties rollingStrategyProperties);

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (Item.IsAir)
			{
				return;
			}

			RecalculateStack();
		}

		public override void ChangeItem(int type, Item item = null)
		{
			base.ChangeItem(type, item);
			RecalculateStack();
		}

		public void RecalculateStack()
		{
			Item.stack = Main.LocalPlayer.inventory.CountItemStack(Item.type, true);
			Item.stack = (int)MathHelper.Clamp(Item.stack, 0f, 999f);
			if (Item.stack <= 0)
			{
				Item.TurnToAir();
				OnItemChange?.Invoke(Item);
			}
		}
	}
}
