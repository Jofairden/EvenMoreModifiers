using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Ext;
using Loot.Soulforging;
using Loot.UI.Common;
using Loot.UI.Common.Controls.Button;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI.Tabs.Soulforging
{
	internal class GuiSoulforgeTab : GuiTab
	{
		public override string Header => "Soulforging";
		public override int GetPageHeight()
		{
			return 400;
		}

		private GuiSoulgauge Soulgauge;
		private GuiInteractableItemButton ItemButton;
		private GuiImageButton CraftButton;
		private GuiInteractableItemButton SoulButton;

		private GuiArrowButton ArrowLeft;
		private GuiArrowButton ArrowRight;
		private UIText Paginator;

		private int CurrentPage = 1;
		private int MaxPages = 1;
		private const int MAX_ROWS_PER_PAGE = 7;
		private bool ShouldUpdate = true;

		public override void OnInitialize()
		{
			base.OnInitialize();

			Soulgauge = new GuiSoulgauge();
			Soulgauge.OnInitialize();
			TabFrame.Append(Soulgauge);

			ItemButton = new GuiInteractableItemButton(
				GuiButton.ButtonType.Parchment,
				hintTexture: ModContent.GetTexture("Terraria/Item_24"),
				hintText: "Place an item here"
			);
			ItemButton.Top.Pixels = Soulgauge.Height.Pixels / 4f;
			ItemButton.RightOf(Soulgauge);
			ItemButton.Left.Pixels += ItemButton.Width.Pixels - GuiTab.PADDING * 1.5f;
			TabFrame.Append(ItemButton);

			CraftButton = new GuiImageButton(GuiButton.ButtonType.None, ModContent.GetTexture("Terraria/UI/Craft_Toggle_3"));
			CraftButton.Top.Pixels = ItemButton.Top.Pixels;
			CraftButton.RightOf(ItemButton);
			TabFrame.Append(CraftButton);

			SoulButton = new GuiInteractableItemButton(
				GuiButton.ButtonType.Parchment,
				hintTexture: Assets.Textures.PlaceholderTexture,
				hintText: "Place a soul here"
			);
			SoulButton.Top.Pixels = ItemButton.Top.Pixels;
			SoulButton.RightOf(CraftButton);
			TabFrame.Append(SoulButton);

			ArrowLeft = new GuiArrowButton(GuiArrowButton.ArrowDirection.LEFT)
			{
				HoverText = "Previous",
				Top = new StyleDimension(24, 1.0f)
			};
			ArrowLeft.WhenClicked += (evt, element, btn) =>
			{
				CurrentPage--;
				ShouldUpdate = true;
			};
			ArrowRight = new GuiArrowButton(GuiArrowButton.ArrowDirection.RIGHT)
			{
				HoverText = "Next",
				Left = new StyleDimension(-15, 0f),
				Top = new StyleDimension(24, 1.0f)
			}.RightOf(ArrowLeft);
			ArrowRight.WhenClicked += (evt, element, btn) =>
			{
				CurrentPage++;
				ShouldUpdate = true;
			};
			TabFrame.Append(ArrowLeft);
			TabFrame.Append(ArrowRight);

			Paginator = new UIText("1/1")
			{
				Top = new StyleDimension(24, 1.0f)
			}.LeftOf(ArrowLeft);
			Paginator.Left.Pixels = -(ArrowLeft.Width.Pixels - 25) / 2f;
			TabFrame.Append(Paginator);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (ShouldUpdate)
			{
				CalculateRubeRows();
				UpdateActiveRows();
				ShouldUpdate = false;
			}
		}

		public List<Item> GetAvailableCubes()
		{
			return Cubes.Where(item => item.stack > 0).ToList();
		}

		private List<Item> Cubes = new List<Item>();
		private List<CubeCraftRow> CubeRows = new List<CubeCraftRow>();
		private List<CubeCraftRow> ActiveRows = new List<CubeCraftRow>();

		private void CalculateRubeRows()
		{
			ActiveRows.ForEach(x => TabFrame.RemoveChild(x));
			CubeRows.Clear();

			var world = ModContent.GetInstance<LootEssenceWorld>();
			var count = world.UnlockedCubes.Count;
			MaxPages = (int)Math.Ceiling(count / (float)MAX_ROWS_PER_PAGE);
			ArrowRight.CanBeClicked = MaxPages > 1 && CurrentPage != MaxPages;
			ArrowLeft.CanBeClicked = MaxPages > 1 && CurrentPage != 1;
			Paginator.SetText($"{CurrentPage}/{MaxPages}");

			foreach (var cube in world.UnlockedCubes)
			{
				var row = new CubeCraftRow(cube);
				row.Activate();
				row.UpdateText();
				CubeRows.Add(row);
				row.OnCubeUpdate += item =>
				{
					Cubes.First(x => x.type == item.type).stack = item.stack;
				};

				if (Cubes.All(item => item.type != cube))
				{
					var item = new Item();
					item.SetDefaults(cube);
					item.stack = 0;
					Cubes.Add(item);
				}
				else
				{
					row.Cube = Cubes.First(x => x.type == cube);
					row.CubeButton.ChangeItem(0, row.Cube);
				}
			}
		}

		private void UpdateActiveRows()
		{
			ActiveRows.Clear();
			UIElement previous = Soulgauge;

			foreach (var row in CubeRows.Skip((CurrentPage - 1) * MAX_ROWS_PER_PAGE).Take(MAX_ROWS_PER_PAGE).ToList())
			{
				row.Cube = Cubes.First(item => item.type == row.Type);
				row.Below(previous);
				TabFrame.Append(row);
				ActiveRows.Add(row);
				previous = row;
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			ShouldUpdate = true;
		}

		public override void GiveBackItems()
		{
			CubeRows.ForEach(row =>
			{
				if (!row.CubeButton.Item.IsAir)
				{
					row.CubeButton.Item.noGrabDelay = 0;
					Main.LocalPlayer.GetItem(Main.myPlayer, row.CubeButton.Item.Clone());
					row.CubeButton.Item.TurnToAir();
					row.Cube.TurnToAir();
				}
			});
		}
	}
}
