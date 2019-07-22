using Loot.Ext;
using Loot.UI.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI
{
	internal class GuiHeader : UIElement
	{
		private string _headerText;

		public int GetOffset() => Assets.Textures.GUI.HeaderTexture.Height;

		public void SetHeader(string newText)
		{
			_headerText = newText;
		}

		public override void OnInitialize()
		{
			Width.Set(Assets.Textures.GUI.HeaderTexture.Width, 0);
			Height.Set(Assets.Textures.GUI.HeaderTexture.Height, 0);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var parentBounds = GetOuterDimensions().ToRectangle();

			var _header = Assets.Textures.GUI.HeaderTexture;
			spriteBatch.Draw(
				_header,
				_header.BoundsFromParent(parentBounds),
				Color.White
			);

			var _decoration = Assets.Textures.GUI.SkullDecorationTexture;
			var decorationBounds = _decoration.BoundsFromParent(parentBounds);
			decorationBounds.Offset((int)(Width.Pixels * 0.5f - _decoration.Width * 0.5f), (int)(-_decoration.Height * 0.5f));

			spriteBatch.Draw(
				_decoration,
				decorationBounds,
				Color.White
			);

			var textPos = GetOuterDimensions().Center();
			var measure = Main.fontMouseText.MeasureString(_headerText);
			textPos.X -= measure.X * 0.5f;
			textPos.Y -= measure.Y * 0.3725f;

			Utils.DrawBorderStringFourWay(
				spriteBatch,
				Main.fontMouseText,
				_headerText,
				textPos.X,
				textPos.Y,
				Color.White,
				Color.Black,
				Vector2.Zero
			);
		}
	}
}
