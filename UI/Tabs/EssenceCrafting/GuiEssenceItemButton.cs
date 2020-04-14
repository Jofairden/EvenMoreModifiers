using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Api.Ext;
using Loot.UI.Common.Controls.Button;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Tabs.EssenceCrafting
{
	class GuiEssenceItemButton : GuiInteractableItemButton
	{
		internal GuiEssenceItemButton(GuiButton.ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			RightClickFunctionalityEnabled = false;
			HintOnHover = " (click to take item)";
		}

		public override bool CanTakeItem(Item givenItem) => base.CanTakeItem(givenItem) && givenItem.IsModifierRollableItem();

		public override void PreOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!Item.IsAir)
			{
				//LootModItem.GetInfo(Item).SlottedInCubeUI = false;
			}
		}

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (!Item.IsAir)
			{
				//LootModItem.GetInfo(Item).SlottedInCubeUI = true;
			}
		}
	}
}
