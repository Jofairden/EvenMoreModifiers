using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Essences;
using Loot.UI.Tabs.CraftingTab;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Loot.UI.Tabs.EssenceCrafting
{
	internal class GuiEssenceButton : CraftingComponentButton
	{
		public GuiEssenceButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
		}

		public override bool CanTakeItem(Item givenItem)
			=> givenItem.modItem is EssenceItem;

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties rollingStrategyProperties)
			=> ((EssenceItem) Item.modItem)?.GetRollingStrategy(item, rollingStrategyProperties) ?? RollingUtils.Strategies.Default;

	}
}
