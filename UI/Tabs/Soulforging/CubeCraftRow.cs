using System;
using Loot.Api.Cubes;
using Loot.Ext;
using Loot.Soulforging;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Common.Controls.Panel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI.Tabs.Soulforging
{
	class CubeCraftRow : GuiFramedElement
	{
		internal int Type;
		internal Item Cube;
		internal Action<Item> OnCubeUpdate;

		private int CraftingCost;
		internal CubeCraftButton CubeButton;
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
			Type = type;
			Cube = new Item();
			Cube.SetDefaults(type);
			Cube.stack = 0;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			CubeButton = new CubeCraftButton(
				GuiButton.ButtonType.StoneInnerBevel,
				hintTexture: Main.itemTexture[Type]
			);
			CubeButton.Item = Cube;
			CubeButton.OnClick += CubeButtonOnOnClick;
			Frame.Append(CubeButton);

			Panel = new GuiTextPanel();
			Panel.RightOf(CubeButton);
			Frame.Append(Panel);

			CraftButton = new GuiImageButton(GuiButton.ButtonType.None, ModContent.GetTexture("Terraria/UI/Craft_Toggle_3"));
			CraftButton.Left.Percent = 1.0f;
			CraftButton.Left.Pixels = -CraftButton.Width.Pixels - GuiTab.PADDING;
			CraftButton.OnClick += CraftCube;
			Panel.Append(CraftButton);
		}

		private void CubeButtonOnOnClick(UIMouseEvent evt, UIElement listeningelement)
		{
			if (CubeButton.Item.stack > 0 && Main.mouseItem.IsAir)
			{
				Main.mouseItem = Cube.Clone();
				Cube.stack = 0;
				CubeButton.Item = Cube.Clone();
				OnCubeUpdate?.Invoke(Cube);
			}
		}

		private void CraftCube(UIMouseEvent evt, UIElement listeningelement)
		{
			var info = Main.LocalPlayer.GetModPlayer<LootEssencePlayer>();
			if (info.Essence >= CraftingCost)
			{
				Main.PlaySound(SoundID.Item37, -1, -1);
				Cube.stack++;
				CubeButton.Item = Cube.Clone();
				info.UseEssence(CraftingCost);
				OnCubeUpdate?.Invoke(Cube);
			}
		}
	}
}
