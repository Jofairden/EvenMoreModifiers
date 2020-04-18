using System;
using Loot.Ext;
using Loot.UI.Common;
using Loot.UI.Tabs.Cubing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Loot.Tiles
{
	internal sealed class MysteriousWorkbench : ModTile
	{
		private const int Size = 16;
		private const int Padding = 2;

		public override void SetDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSpelunker[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			//TileID.Sets.HasOutlines[Type] = true;

			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateWidth = Size;
			TileObjectData.newTile.CoordinatePadding = Padding;
			TileObjectData.newTile.CoordinateHeights = new int[] {Size, Size, Size};
			//TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<DeconstructorTE>().Hook_AfterPlacement, -1, 0, true);
			TileObjectData.newTile.Origin = new Point16(2, 2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;

			TileObjectData.addTile(Type);
			//AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("MysteriousWorkbench");
			AddMapEntry(new Color(50, 50, 50), name);

			disableSmartCursor = true;
		}

		public override void MouseOver(int i, int j)
		{
			if (Main.LocalPlayer.mouseInterface) return;
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			player.noThrow = 2;
			player.showItemIcon = true;
			// player.showItemIconText = "MysteriousWorkbench";
			player.showItemIcon2 = ModContent.ItemType<MysteriousWorkbenchItem>();
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}

		public override bool NewRightClick(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;

			if (tile.type == Type && Main.LocalPlayer.Distance(new Point16(i, j).ToWorldCoordinates()) <= 15 * 16f)
			{
				Loot.Instance.GuiState.ToggleUI(Loot.Instance.GuiInterface);
				return true;
			}

			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Color useColor = Color.White;
			Tile tile = Main.tile[i, j];

			if (tile.type == Type)
			{
				var sine = (float) Math.Sin(Main.essScale * 0.50f);
				r = 0.05f + 0.35f * sine * useColor.R * 0.01f;
				g = 0.05f + 0.35f * sine * useColor.G * 0.01f;
				b = 0.05f + 0.35f * sine * useColor.B * 0.01f;
			}
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			if (Loot.Instance.GuiState.Visible && Main.LocalPlayer.Distance(new Point16(i, j).ToWorldCoordinates()) > 15 * 16f)
			{
				Loot.Instance.GuiState.ToggleUI(Loot.Instance.GuiInterface);
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (!Main.dedServ && Loot.Instance.GuiState.Visible)
			{
				// Kill UI
				Loot.Instance.GuiState.ToggleUI(Loot.Instance.GuiInterface);
			}

			Item.NewItem(i * 16, j * 16, 20, 28, ModContent.ItemType<MysteriousWorkbenchItem>());
			//mod.GetTileEntity<DeconstructorTE>().Kill(i, j);
		}

		private float frame;

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			var tile = Main.tile[i, j];
			// Only draw from top left tile
			if (tile.type == Type
			    && tile.IsTopLeftFrame())
			{
				var top = tile.GetTopLeftFrame(i, j, Size, Padding);

				if (Loot.Instance.GuiState.Visible
				    && Loot.Instance.GuiState.GetCurrentTab() is GuiCubingTab tab
				    && !tab.ComponentButton.Item.IsAir)
				{
					Vector2 zero = Main.drawToScreen
						? Vector2.Zero
						: new Vector2(Main.offScreenRange, Main.offScreenRange);

					Texture2D animTexture = Assets.Textures.GUI.LunarCubeTexture;
					const int frameWidth = 20;
					const int frameHeight = 28;
					Vector2 offset = new Vector2(36f, 8f); // offset 2.5 tiles horizontal, 0.5 tile vertical
					Vector2 position = new Vector2(i, j) * 16f - Main.screenPosition + offset;
					Vector2 origin = new Vector2(frameHeight, frameWidth) * 0.5f;
					// tiles draw every 5 ticks, so we can safely increment here
					frame = (frame + 0.75f) % 8;
					spriteBatch.Draw(animTexture, position + zero,
						new Rectangle(0, frameHeight * (int) frame, frameWidth, frameHeight), Color.White, 0f, origin, 1f,
						SpriteEffects.None, 0f);
				}
			}
		}
	}
}
