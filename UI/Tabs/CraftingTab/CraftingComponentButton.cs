using Loot.Api.Strategy;
using Loot.UI.Common.Controls.Button;
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
		public CraftingComponentLink Link;

		internal CraftingComponentButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			RightClickFunctionalityEnabled = false;
			TakeUserItemOnClick = false;
			HintOnHover = " (click to unslot)";
		}

		public abstract RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties rollingStrategyProperties);

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{

		}

		public override void ChangeItem(int type, Item item = null)
		{
			if (item != null)
			{
				SetLink(new CraftingComponentLink(item, CraftingComponentLink.ComponentSource.Dynamic));
			}
			else
			{
				var component = new Item();
				component.SetDefaults(type);
				SetLink(new CraftingComponentLink(component, CraftingComponentLink.ComponentSource.Dynamic));
			}
		}

		public void SetLink(CraftingComponentLink link)
		{
			if (link == null)
			{
				Item.TurnToAir();
				Link = null;
			}
			else
			{
				Link = link;
				Item = Link.Component.Clone();
			}
			OnItemChange?.Invoke(Item);
		}
	}
}
