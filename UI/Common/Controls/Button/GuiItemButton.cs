using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common.Controls.Button
{
	internal class GuiItemButton : GuiButton
	{
		public string HintText { get; protected internal set; }
		public Texture2D HintTexture { get; protected internal set; }
		public string HintOnHover { get; protected internal set; }
		public bool DrawBackground { get; protected internal set; } = true;
		public bool DrawStack { get; protected internal set; } = true;
		public bool ShowOnlyHintOnHover { get; protected internal set; } = false;
		public float DrawScale { get; protected internal set; } = 1f;

		public bool DynamicScaling
		{
			get;
			protected internal set;
		} = true;
		public Color? DrawColor
		{
			get;
			protected internal set;
		} = null;

		public Item Item
		{
			get;
			protected internal set;
		}

		internal GuiItemButton(ButtonType buttonType,
			int netId = 0, int stack = 0, Texture2D hintTexture = null,
			string hintText = null, string hintOnHover = null) : base(buttonType)
		{
			Item = new Item();
			Item.netDefaults(netId);
			Item.stack = stack;
			HintTexture = hintTexture;
			HintText = hintText;
			HintOnHover = hintOnHover;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (DrawBackground)
			{
				base.DrawSelf(spriteBatch);
			}

			Texture2D texture2D;
			CalculatedStyle innerDimensions = GetInnerDimensions();
			Color drawColor;

			if (HintTexture != null && Item.IsAir)
			{
				texture2D = HintTexture;
				drawColor = Color.LightGray * 0.5f;
				if (IsMouseHovering)
				{
					Main.hoverItemName = HintText ?? string.Empty;
				}
			}
			else if (Item.IsAir)
			{
				return;
			}
			else
			{
				texture2D = Main.itemTexture[Item.type];
				drawColor = DrawColor ?? Item.GetAlpha(Color.White);
				if (IsMouseHovering)
				{
					if (ShowOnlyHintOnHover)
					{
						Main.hoverItemName = HintOnHover;
					}
					else
					{
						Main.hoverItemName = Item.Name;
						Main.HoverItem = Item.Clone();
						Main.HoverItem.SetNameOverride(
							$"{Main.HoverItem.Name}{Main.HoverItem.modItem?.mod.Name.Insert((int)Main.HoverItem.modItem?.mod.Name.Length, "]").Insert(0, " [")}{HintOnHover ?? ""}");
					}
				}
			}

			var frame =
				!Item.IsAir && Main.itemAnimations[Item.type] != null
					? Main.itemAnimations[Item.type].GetFrame(texture2D)
					: texture2D.Frame();

			float drawScale = DrawScale;
			if (DynamicScaling)
			{
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
			if (Item.modItem == null
				|| Item.modItem.PreDrawInInventory(spriteBatch, drawPosition, frame, drawColor, drawColor, Vector2.Zero, drawScale))
			{
				spriteBatch.Draw(texture2D, drawPosition, frame, drawColor, 0f,
					Vector2.Zero, drawScale, SpriteEffects.None, 0f);

				if (Item?.color != default(Color))
				{
					spriteBatch.Draw(texture2D, drawPosition, frame, drawColor, 0f,
						Vector2.Zero, drawScale, SpriteEffects.None, 0f);
				}
			}

			Item.modItem?.PostDrawInInventory(spriteBatch, drawPosition, frame, drawColor, drawColor, Vector2.Zero, drawScale);

			if (!DrawStack || !(Item?.stack > 1))
			{
				return;
			}

			string stack = Math.Min(9999, Item.stack).ToString();
			Vector2 stackMeasuring = Main.fontItemStack.MeasureString(stack);
			var corner = innerDimensions.Center() + new Vector2(WIDTH / 2f, HEIGHT / 2f);

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
