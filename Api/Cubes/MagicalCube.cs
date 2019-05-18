using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Loot.Api.Cubes
{
	/// <summary>
	/// Defines a magical cube
	/// A magical cube is used to change modifiers on an item
	/// </summary>
	public abstract class MagicalCube : ModItem
	{
		protected abstract string CubeName { get; }
		protected virtual Color? OverrideNameColor => null;
		protected virtual TooltipLine ExtraTooltip => null;

		public sealed override void SetDefaults()
		{
			item.Size = new Vector2(36);
			item.maxStack = 999;
			item.consumable = false;
			SafeDefaults();
		}

		public override void SetStaticDefaults()
		{
			SafeStaticDefaults();
		}

		protected virtual void SafeStaticDefaults()
		{
		}

		protected virtual void SafeDefaults()
		{
		}

		public override bool CanRightClick() => !PlayerInput.WritingText && Main.hasFocus && Main.keyState.IsKeyDown(Keys.LeftControl);

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			base.ModifyTooltips(tooltips);

			var tts = tooltips.Where(x => x.mod.Equals("Terraria"));

			if (OverrideNameColor != null)
			{
				var itemName = tts.FirstOrDefault(x => x.Name.Equals("ItemName"));
				if (itemName != null)
				{
					itemName.overrideColor = OverrideNameColor.Value;
				}
			}

			if (ExtraTooltip != null)
			{
				var desc = tts.Last(x => x.Name.StartsWith("Tooltip"));
				if (desc != null)
				{
					tooltips.Insert(tooltips.IndexOf(desc) + 1, ExtraTooltip);
				}
			}
		}

		private const int PADDING_FOR_BOX = 2;

		// Highlight important parts
		public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
		{
			if (line.mod.Equals("Terraria") && line.Name.Equals("ItemName")
			    || line.mod.Equals("Loot") && line.Name.EndsWith("::Add_Box"))
			{
				var stringSize = ChatManager.GetStringSize(Main.fontMouseText, line.text, Vector2.One);
				int widthForBox = (int) stringSize.X + PADDING_FOR_BOX * 2;
				int heightForBox = (int) Main.fontMouseText.MeasureString(line.text).Y + PADDING_FOR_BOX * 2;

				Vector2 drawPosForBox = new Vector2(line.X - PADDING_FOR_BOX, line.Y - PADDING_FOR_BOX * 2);
				Rectangle drawRectForBox = new Rectangle((int) drawPosForBox.X, (int) drawPosForBox.Y, widthForBox, heightForBox);
				Main.spriteBatch.Draw(Main.magicPixel, drawRectForBox, Main.mouseTextColorReal);

				if (line.Name.Equals("ItemName"))
				{
					line.X += (int) (widthForBox / 2 - PADDING_FOR_BOX) - (int) line.font.MeasureString(line.text).X / 2;
				}
			}

			return true;
		}
	}
}
