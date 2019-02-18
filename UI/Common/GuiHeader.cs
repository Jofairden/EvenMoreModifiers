using Loot.Ext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common
{
	internal class GuiHeader : UIElement
	{
		private Texture2D _texture;
		private Texture2D _decoration;
		private string _headerText;

		public int GetOffset() => _texture.Height;

		public void SetHeader(string newText)
		{
			_headerText = newText;
		}

		public GuiHeader()
		{
			_texture = Loot.Instance.GetTexture("UI/Common/GuiHeader");
			_decoration = Loot.Instance.GetTexture("UI/Common/HeaderSkull");
		}

		public override void OnInitialize()
		{
			Width.Set(_texture.Width, 0);
			Height.Set(_texture.Height, 0);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var parentBounds = GetOuterDimensions().ToRectangle();

			spriteBatch.Draw(
				_texture,
				_texture.BoundsFromParent(parentBounds),
				Color.White
			);

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
