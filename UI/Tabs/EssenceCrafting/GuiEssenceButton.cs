using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Api.Cubes;
using Loot.Api.Strategy;
using Loot.Essences;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Tabs.Cubing;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Tabs.EssenceCrafting
{
	class GuiEssenceButton : GuiInteractableItemButton
	{
		internal GuiEssenceButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			RightClickFunctionalityEnabled = false;
			TakeUserItemOnClick = false;
			HintOnHover = " (click to unslot)";
		}

		public override bool CanTakeItem(Item givenItem)
			=> givenItem.modItem is EssenceItem;

		public RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties rollingStrategyProperties)
			=> ((EssenceItem)Item.modItem).GetRollingStrategy(item, rollingStrategyProperties);
	}
}
