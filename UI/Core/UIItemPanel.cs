using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI.Core
{
	/*
	 * Ported from my deconstructor mod
	 */
	public class UIItemPanel : UIPanel
	{
		internal const float PANEL_WIDTH = 50f;
		internal const float PANEL_HEIGHT = 50f;
		internal const float PANEL_PADDING = 0f;

		public string HintText { get; protected internal set; }
		public Texture2D HintTexture { get; protected internal set; }
		public string HintOnHover { get; protected internal set; }
		public bool DrawBackgroundPanel { get; protected internal set; } = true;
		public bool DrawStack { get; protected internal set; } = true;
		public bool ShowOnlyHintOnHover { get; protected internal set; } = false;
		public float DrawScale { get; protected internal set; } = 1f;
		public Color? DrawColor { get; protected internal set; } = null;

		public Item item;

		public UIItemPanel(int netID = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null)
		{
			Width.Set(PANEL_WIDTH, 0f);
			Height.Set(PANEL_HEIGHT, 0f);
			SetPadding(PANEL_PADDING);
			item = new Item();
			item.netDefaults(netID);
			item.stack = stack;
			HintTexture = hintTexture;
			HintText = hintText;
			HintOnHover = hintOnHover;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (DrawBackgroundPanel)
			{
				base.DrawSelf(spriteBatch);
			}

			Texture2D texture2D;
			CalculatedStyle innerDimensions = GetInnerDimensions();
			Color drawColor;

			if (HintTexture != null && item.IsAir)
			{
				texture2D = HintTexture;
				drawColor = Color.LightGray * 0.5f;
				if (IsMouseHovering)
				{
					Main.hoverItemName = HintText ?? string.Empty;
				}
			}
			else if (item.IsAir)
			{
				return;
			}
			else
			{
				texture2D = Main.itemTexture[item.type];
				drawColor = DrawColor ?? item.GetAlpha(Color.White);
				if (IsMouseHovering)
				{
					if (ShowOnlyHintOnHover)
					{
						Main.hoverItemName = HintOnHover;
					}
					else
					{
						Main.hoverItemName = item.Name;
						Main.HoverItem = item.Clone();
						//Main.HoverItem.GetGlobalItem<DeconGlobalItem>(TheDeconstructor.instance).addValueTooltip = true;
						//ItemValue value = new ItemValue().SetFromCopperValue(item.value*item.stack);
						Main.HoverItem.SetNameOverride(
							$"{Main.HoverItem.Name}{Main.HoverItem.modItem?.mod.Name.Insert((int) Main.HoverItem.modItem?.mod.Name.Length, "]").Insert(0, " [")}{HintOnHover ?? ""}");
					}
				}
			}

			var frame =
				!item.IsAir && Main.itemAnimations[item.type] != null
					? Main.itemAnimations[item.type].GetFrame(texture2D)
					: texture2D.Frame();

			float drawScale = DrawScale;
			if (frame.Width > innerDimensions.Width
			    || frame.Height > innerDimensions.Width)
			{
				if (frame.Width > frame.Height)
				{
					drawScale = innerDimensions.Width / frame.Width;
				}
				else
				{
					drawScale = innerDimensions.Width / frame.Height;
				}
			}

			var unreflectedScale = drawScale;

			// TODO why was this here?
			//var tmpcolor = Color.White;
			//// 'Breathing' effect
			//ItemSlot.GetItemLight(ref tmpcolor, ref drawScale, item.type);

			Vector2 drawPosition = new Vector2(innerDimensions.X, innerDimensions.Y);

			drawPosition.X += innerDimensions.Width * 1f / 2f - frame.Width * drawScale / 2f;
			drawPosition.Y += innerDimensions.Height * 1f / 2f - frame.Height * drawScale / 2f;

			// TODO globalitem Pre and Post draws?
			if (item.modItem == null
			    || item.modItem.PreDrawInInventory(spriteBatch, drawPosition, frame, drawColor, drawColor, Vector2.Zero, drawScale))
			{
				spriteBatch.Draw(texture2D, drawPosition, frame, drawColor, 0f,
					Vector2.Zero, drawScale, SpriteEffects.None, 0f);

				if (item?.color != default(Color))
				{
					spriteBatch.Draw(texture2D, drawPosition, frame, drawColor, 0f,
						Vector2.Zero, drawScale, SpriteEffects.None, 0f);
				}
			}

			item.modItem?.PostDrawInInventory(spriteBatch, drawPosition, frame, drawColor, drawColor, Vector2.Zero, drawScale);

			if (DrawStack && item?.stack > 1)
			{
				string stack = Math.Min(9999, item.stack).ToString();
				Vector2 stackMeasuring = Main.fontItemStack.MeasureString(stack);
				var corner = innerDimensions.Center() + new Vector2(PANEL_WIDTH / 2f, PANEL_HEIGHT / 2f);

				Utils.DrawBorderStringFourWay(
					spriteBatch,
					Main.fontItemStack,
					stack,
					corner.X - stackMeasuring.X,
					corner.Y - stackMeasuring.Y,
					Color.White,
					Color.Black,
					Vector2.Zero,
					unreflectedScale * 0.8f);
			}
		}
	}
}
