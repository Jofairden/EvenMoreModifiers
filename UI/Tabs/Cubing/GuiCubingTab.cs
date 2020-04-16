using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Cubes;
using Loot.UI.Common;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Tabs.CraftingTab;
using Loot.UI.Tabs.EssenceCrafting;
using Terraria;

namespace Loot.UI.Tabs.Cubing
{
	/// <summary>
	/// The tab allows using a magical cube on the slotted item to modify its potential
	/// </summary>
	internal class GuiCubingTab : CraftingTab<MagicalCube>
	{
		public override string Header => "Cubing";

		internal override CraftingComponentButton GetComponentButton()
		{
			return new GuiCubeButton(
				GuiButton.ButtonType.StoneOuterBevel,
				hintTexture: Assets.Textures.GUI.MagicalCubeTexture,
				hintText: "Place a cube here"
			);
		}

		protected override ModifierContextMethod CraftMethod => ModifierContextMethod.OnCubeReroll;

		protected override Dictionary<string, object> CustomData => new Dictionary<string, object>()
		{
			{
				"Source", "CubeRerollUI"
			}
		};

		public override void OnActivate()
		{
			base.OnActivate();
			var essenceTab = Loot.Instance.GuiState.GetTab<GuiEssenceTab>();
			if (!essenceTab.ItemButton?.Item?.IsAir ?? false)
			{
				ItemButton.ChangeItem(0, essenceTab.ItemButton.Item.Clone());
				essenceTab.ItemButton.Item.TurnToAir();
			}
		}
	}
}
