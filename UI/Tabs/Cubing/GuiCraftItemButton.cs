using System;
using Loot.Api.Ext;
using Loot.UI.Common.Controls.Button;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Tabs.Cubing
{
	internal class GuiCraftItemButton : GuiInteractableItemButton
	{
		public Action PostOnClickAction;
		public Action PreOnClickAction;
		public Func<Item, bool> CanTakeItemAction;

		internal GuiCraftItemButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			RightClickFunctionalityEnabled = false;
			HintOnHover = " (click to take item)";
		}

		public override bool CanTakeItem(Item givenItem) => base.CanTakeItem(givenItem) && givenItem.IsModifierRollableItem() && (CanTakeItemAction?.Invoke(givenItem) ?? true);

		public override void PreOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!Item.IsAir)
			{
				LootModItem.GetInfo(Item).SlottedInUI = false;
			}
			PreOnClickAction?.Invoke();
		}

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!Item.IsAir)
			{
				LootModItem.GetInfo(Item).SlottedInUI = true;
			}
			PostOnClickAction?.Invoke();
		}
	}
}
