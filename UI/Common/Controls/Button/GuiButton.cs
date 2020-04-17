using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.UI.Common.Controls.Button
{
	internal class GuiButton : GuiFramedElement
	{
		protected const int WIDTH = 50;
		protected const int HEIGHT = 50;

		internal enum ButtonType
		{
			StoneInnerBevel,
			StoneOuterBevel,
			Parchment,
			None
		}

		private static Texture2D _texture => Assets.Textures.GUI.ButtonTexture;
		protected ButtonType _buttonType;

		internal GuiButton(ButtonType buttonType) : base(new Vector2(WIDTH, HEIGHT), new Vector2(10, 10))
		{
			_buttonType = buttonType;
		}

		private Rectangle GetDrawRectangle()
		{
			switch (_buttonType)
			{
				case ButtonType.StoneInnerBevel:
					return new Rectangle(0, 0, WIDTH, HEIGHT);
				case ButtonType.StoneOuterBevel:
					return new Rectangle(WIDTH, 0, WIDTH, HEIGHT);
				case ButtonType.Parchment:
				default:
					return new Rectangle(2 * WIDTH, 0, WIDTH, HEIGHT);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			//base.DrawSelf(spriteBatch);
			if (_buttonType != ButtonType.None)
			{
				spriteBatch.Draw(
					_texture,
					GetOuterDimensions().Position(),
					GetDrawRectangle(),
					Color.White
				);
			}

			//#if DEBUG
			//			spriteBatch.Draw(
			//				Main.magicPixel,
			//				GetOuterDimensions().ToRectangle(),
			//				Color.Blue * 0.4f
			//			);
			//#endif
		}
	}
}
