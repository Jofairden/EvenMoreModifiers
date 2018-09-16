using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI
{
	/*
	 * Ported from my deconstructor mod
	 */
	public class UIItemPanel : UIPanel
	{
		internal const float panelwidth = 50f;
		internal const float panelheight = 50f;
		internal const float panelpadding = 0f;

		public string HintText { get; set; }
		public Texture2D HintTexture { get; set; }
		public Item item;
		protected string HintOnHover { get; set; }

		public UIItemPanel(int netID = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null)
		{
			base.Width.Set(panelwidth, 0f);
			base.Height.Set(panelheight, 0f);
			base.SetPadding(panelpadding);
			this.item = new Item();
			this.item.netDefaults(netID);
			this.item.stack = stack;
			this.HintTexture = hintTexture;
			this.HintText = hintText;
			this.HintOnHover = hintOnHover;
		}

		//public virtual void BindItem(DeconEntityInstance instance) { }

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			Texture2D texture2D;
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			Color drawColor;

			if (HintTexture != null
				&& item.IsAir)
			{
				texture2D = HintTexture;
				drawColor = Color.LightGray * 0.5f;
				if (base.IsMouseHovering)
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
				drawColor = this.item.GetAlpha(Color.White);
				if (base.IsMouseHovering)
				{
					Main.hoverItemName = item.Name;
					Main.HoverItem = item.Clone();
					//Main.HoverItem.GetGlobalItem<DeconGlobalItem>(TheDeconstructor.instance).addValueTooltip = true;
					//ItemValue value = new ItemValue().SetFromCopperValue(item.value*item.stack);
					Main.HoverItem.SetNameOverride(
						$"{Main.HoverItem.Name}{Main.HoverItem.modItem?.mod.Name.Insert((int)Main.HoverItem.modItem?.mod.Name.Length, "]").Insert(0, " [")}{(HintOnHover ?? "")}");
				}
			}

			var frame =
					!item.IsAir && Main.itemAnimations[item.type] != null
						? Main.itemAnimations[item.type].GetFrame(texture2D)
						: texture2D.Frame(1, 1, 0, 0);

			float drawScale = 1f;
			if ((float)frame.Width > innerDimensions.Width
				|| (float)frame.Height > innerDimensions.Width)
			{
				if (frame.Width > frame.Height)
				{
					drawScale = innerDimensions.Width / (float)frame.Width;
				}
				else
				{
					drawScale = innerDimensions.Width / (float)frame.Height;
				}
			}

			var unreflectedScale = drawScale;
			var tmpcolor = Color.White;
			// 'Breathing' effect
			ItemSlot.GetItemLight(ref tmpcolor, ref drawScale, item.type);

			Vector2 drawPosition = new Vector2(innerDimensions.X, innerDimensions.Y);

			drawPosition.X += (float)innerDimensions.Width * 1f / 2f - (float)frame.Width * drawScale / 2f;
			drawPosition.Y += (float)innerDimensions.Height * 1f / 2f - (float)frame.Height * drawScale / 2f;

			//todo: globalitem?
			if (item.modItem == null
				|| item.modItem.PreDrawInInventory(spriteBatch, drawPosition, frame, drawColor, drawColor, Vector2.Zero, drawScale))
			{
				spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(frame), drawColor, 0f,
					Vector2.Zero, drawScale, SpriteEffects.None, 0f);

				if (this.item?.color != default(Color))
				{
					spriteBatch.Draw(texture2D, drawPosition, new Rectangle?(frame), drawColor, 0f,
						Vector2.Zero, drawScale, SpriteEffects.None, 0f);
				}
			}

			item.modItem?.PostDrawInInventory(spriteBatch, drawPosition, frame, drawColor, drawColor, Vector2.Zero, drawScale);

			// Draw stack count
			if (this.item?.stack > 1)
			{
				Utils.DrawBorderStringFourWay(
					spriteBatch,
					Main.fontItemStack,
					Math.Min(9999, item.stack).ToString(),
					innerDimensions.Position().X + 10f,
					innerDimensions.Position().Y + 26f,
					Color.White,
					Color.Black,
					Vector2.Zero,
					unreflectedScale * 0.8f);
			}
		}

	}
}