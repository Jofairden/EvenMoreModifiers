using Loot.UI.Common.Controls.Button;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Tabs.Soulforging
{
	class CubeCraftButton : GuiInteractableItemButton
	{
		internal CubeCraftButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			PerformRegularClickInteraction = false;
			ShowStackFrom = 1;
		}

		public override bool CanTakeItem(Item givenItem) => false;
	}
}
