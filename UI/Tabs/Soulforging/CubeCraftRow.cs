using Loot.Api.Cubes;
using Loot.Ext;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Common.Controls.Panel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.UI.Tabs.Soulforging
{
	class CubeCraftRow : GuiFramedElement
	{
		private Item Cube;
		private int CraftingCost;
		private GuiInteractableItemButton CubeButton;
		private GuiTextPanel Panel;
		private GuiImageButton CraftButton;

		public void UpdateText()
		{
			CraftingCost = ((MagicalCube)(Cube.modItem))?.EssenceCraftCost ?? 0;
			Panel?.UpdateText($"{CraftingCost} essence cost");
			Panel?.SetHoverText($"Crafting one {Cube.HoverName} costs {CraftingCost} essence");
		}

		public CubeCraftRow(int type) : base(new Vector2(400, 55), new Vector2(0, 0))
		{
			Cube = new Item();
			Cube.SetDefaults(type);
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			CubeButton = new GuiInteractableItemButton(
				GuiButton.ButtonType.StoneOuterBevel,
				hintTexture: Main.itemTexture[Cube.type]
			);
			Frame.Append(CubeButton);

			Panel = new GuiTextPanel();
			Panel.RightOf(CubeButton);
			Frame.Append(Panel);

			CraftButton = new GuiImageButton(GuiButton.ButtonType.None, ModContent.GetTexture("Terraria/UI/Craft_Toggle_3"));
			CraftButton.Left.Percent = 1.0f;
			CraftButton.Left.Pixels = -CraftButton.Width.Pixels - GuiTab.PADDING;
			Panel.Append(CraftButton);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (IsMouseHovering)
			{
				if (CubeButton.IsMouseHovering)
				{
					Main.hoverItemName = Cube.HoverName;
				}
			}
		}
	}
}
