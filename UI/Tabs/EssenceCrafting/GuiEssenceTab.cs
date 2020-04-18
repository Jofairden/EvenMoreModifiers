using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Essences;
using Loot.UI.Common;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Tabs.CraftingTab;
using Loot.UI.Tabs.Cubing;

namespace Loot.UI.Tabs.EssenceCrafting
{
	internal class GuiEssenceTab : CraftingTab<EssenceItem>
	{
		public override string Header => "Essence Crafting";

		internal override CraftingComponentButton GetComponentButton()
		{
			return new GuiEssenceButton(
				GuiButton.ButtonType.StoneOuterBevel,
				hintTexture: Assets.Textures.PlaceholderTexture,
				hintText: "Place an essence here"
			);
		}

		protected override ModifierContextMethod CraftMethod => ModifierContextMethod.OnEssenceReroll;

		protected override Dictionary<string, object> CustomData => new Dictionary<string, object>()
		{
			{
				"Source", "EssenceRerollUI"
			}
		};

		public override void OnActivate()
		{
			base.OnActivate();
			var cubingTab = Loot.Instance.GuiState.GetTab<GuiCubingTab>();
			if (!cubingTab.ItemButton?.Item?.IsAir ?? false)
			{
				ItemButton.ChangeItem(0, cubingTab.ItemButton.Item.Clone());
				cubingTab.ItemButton.Item.TurnToAir();
			}
		}
	}
}
