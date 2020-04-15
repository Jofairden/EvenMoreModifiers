using Loot.Api.Cubes;
using Loot.Api.Strategy;
using Loot.UI.Tabs.CraftingTab;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Loot.UI.Tabs.Cubing
{
	internal class GuiCubeButton : CraftingComponentButton
	{
		public GuiCubeButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
		}

		public override bool CanTakeItem(Item givenItem)
		{
			return givenItem.modItem is MagicalCube;
		}

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties rollingStrategyProperties)
		{
			return ((RerollingCube)Item.modItem).GetRollingStrategy(item, rollingStrategyProperties);
		}
	}
}
