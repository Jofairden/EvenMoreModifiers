using Loot.Ext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common
{
	internal class GuiCloseButton : UIElement
	{
		private readonly Texture2D _btnTexture;
		private readonly Texture2D _chainsTexture;

		public GuiCloseButton()
		{
			_btnTexture = Loot.Instance.GetTexture("UI/Common/GuiCloseButton");
			_chainsTexture = Loot.Instance.GetTexture("UI/Common/GuiCloseButtonChains");
		}

		public override void OnInitialize()
		{
			Width.Set(_btnTexture.Width, 0);
			Height.Set(_btnTexture.Height, 0);
			Top.Set(20 + _chainsTexture.Height, 0);
			Left.Set(422, 0);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var parentBounds = GetOuterDimensions().ToRectangle();
			Rectangle BoundsWithOffset(Vector2 bounds, Point off)
			{
				return new Rectangle(parentBounds.X + off.X, parentBounds.Y + off.Y, (int)bounds.X, (int)bounds.Y);
			}

			spriteBatch.Draw(
				_btnTexture,
				parentBounds,
				Color.White
			);

			Point offset = new Point(
				0,
				-_chainsTexture.Height
			);

			Rectangle rectangle = _chainsTexture.BoundsFromParent(BoundsWithOffset(_chainsTexture.Size(), offset));
			spriteBatch.Draw(
				_chainsTexture,
				rectangle,
				Color.White
			);

			if (parentBounds.Contains(Main.MouseScreen.ToPoint()))
			{
				Main.hoverItemName = "Close";
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					Loot.Instance.GuiState.ToggleUI(Loot.Instance.GuiInterface);
				}
			}
		}
	}
}
