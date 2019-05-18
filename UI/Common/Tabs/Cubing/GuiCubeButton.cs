using Loot.Core.Cubes;
using Loot.Core.System.Strategy;
using Loot.Ext;
using Loot.UI.Common.Controls.Button;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common.Tabs.Cubing
{
	internal class GuiCubeButton : GuiInteractableItemButton
	{
		internal GuiCubeButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			RightClickFunctionalityEnabled = false;
			TakeUserItemOnClick = false;
			HintOnHover = " (click to unslot)";
		}

		public override bool CanTakeItem(Item givenItem)
			=> givenItem.modItem is RerollingCube;

		public override void PostOnClick(UIMouseEvent evt, UIElement e)
		{
			if (Item.IsAir)
			{
				return;
			}

			RecalculateStack();
			if (Loot.Instance.GuiState.GetTab() is GuiCubingTab cubingTab)
			{
				cubingTab._guiCubeSelector.DetermineAvailableCubes();
			}
		}

		public void ChangeItem(int type)
		{
			Item.SetDefaults(type);
			RecalculateStack();
			if (Loot.Instance.GuiState.GetTab() is GuiCubingTab cubingTab)
			{
				cubingTab._guiCubeSelector.DetermineAvailableCubes();
			}
		}

		public IRollingStrategy<RollingStrategyContext> GetRollingStrategy(Item item, RollingStrategyProperties rollingStrategyProperties) 
			=> ((RerollingCube)Item.modItem).GetRollingStrategy(item, rollingStrategyProperties);

		public void RecalculateStack()
		{
			Item.stack = Main.LocalPlayer.inventory.CountItemStack(Item.type, true);
			Item.stack = (int)MathHelper.Clamp(Item.stack, 0f, 999f);
			if (Item.stack <= 0)
			{
				Item.TurnToAir();
			}
		}
	}
}
