using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common.Controls.Button
{
	internal class GuiArrowButton : UIElement
	{
		public enum ArrowDirection
		{
			LEFT,
			RIGHT,
			DEFAULT = RIGHT
		}

		private static Texture2D _btnTexture => Assets.Textures.GUI.ArrowButtonTexture;
		public bool CanBeClicked;
		public string HoverText;
		private readonly ArrowDirection _arrowDirection;

		public delegate void WhenClickedEvent(UIMouseEvent evt, UIElement listeningElement, GuiArrowButton btn);
		public event WhenClickedEvent WhenClicked;

		private SpriteEffects GetSpriteEffects()
		{
			switch (_arrowDirection)
			{
				default:
				case ArrowDirection.RIGHT:
					return SpriteEffects.None;
				case ArrowDirection.LEFT:
					return SpriteEffects.FlipHorizontally;
			}
		}

		public GuiArrowButton(ArrowDirection direction)
		{
			_arrowDirection = direction;
			Width.Set(_btnTexture.Width, 0);
			Height.Set(_btnTexture.Height, 0);
		}

		public override void OnInitialize()
		{
			OnClick += (evt, element) =>
			{
				if (CanBeClicked)
				{
					WhenClicked?.Invoke(evt, element, this);
				}
			};
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Color drawColor = Color.Gray * 0.75f;

			var parentBounds = GetOuterDimensions().ToRectangle();

			if (parentBounds.Contains(Main.MouseScreen.ToPoint()))
			{
				if (CanBeClicked)
				{
					drawColor = Color.White;
					Main.hoverItemName = HoverText;
					// This is needed because the current arrows are outside
					// the UI frame causing vanilla mouse behavior to not register
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Click(new UIMouseEvent(this, Main.MouseScreen));
					}
				}
			}

			spriteBatch.Draw(
				_btnTexture,
				parentBounds,
				null, drawColor, 0f, Vector2.Zero,
				GetSpriteEffects(), 0f
			);
		}
	}
}
