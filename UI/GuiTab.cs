using Loot.Ext;
using Loot.UI.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI
{
	internal abstract class GuiTab : UIElement
	{
		internal const int SPACING = GuiUtils.SPACING;
		internal const int PADDING = GuiUtils.PADDING;

		public abstract string Header { get; }
		private int _pageHeight;

		internal int _GetPageHeight()
		{
			_pageHeight = GetPageHeight();
			return _pageHeight;
		}

		public abstract int GetPageHeight();

		public virtual void OnShow()
		{

		}

		public virtual void OnHide()
		{

		}
		internal virtual void ToggleUI(bool visible)
		{
		}

		protected UIElement TabFrame;
		protected static Texture2D _topTexture => Assets.Textures.GUI.PanelTopTexture;
		protected static Texture2D _middleTexture => Assets.Textures.GUI.PanelTileTexture;
		protected static Texture2D _bottomTexture => Assets.Textures.GUI.PanelBottomTexture;

		public int TotalHeight => _topTexture.Height + _pageHeight + _bottomTexture.Height;

		public override void OnInitialize()
		{
			_GetPageHeight();

			Height.Set(_pageHeight + _topTexture.Height + _bottomTexture.Height, 0);
			Width.Set(422, 0);

			TabFrame = new UIElement
			{
				Top = new StyleDimension(SPACING, 0),
				Left = new StyleDimension(SPACING, 0),
				Width = new StyleDimension(Width.Pixels - SPACING * 2, 0),
				Height = new StyleDimension(Height.Pixels - SPACING * 2, 0)
			};
			Append(TabFrame);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var parentBounds = GetOuterDimensions().ToRectangle();

			Rectangle BoundsWithOffset(Vector2 bounds, Point off)
			{
				return new Rectangle(parentBounds.X + off.X, parentBounds.Y + off.Y, (int)bounds.X, (int)bounds.Y);
			}

			spriteBatch.Draw(
				_topTexture,
				_topTexture.BoundsFromParent(parentBounds),
				Color.White
			);

			Point offset = new Point(
				0,
				_topTexture.Height
			);

			for (int i = 0; i < _pageHeight; i++)
			{
				spriteBatch.Draw(
					_middleTexture,
					_middleTexture.BoundsFromParent(BoundsWithOffset(_middleTexture.Size(), offset)),
					Color.White
				);
				offset.Y += 1;
			}

			spriteBatch.Draw(
				_bottomTexture,
				_bottomTexture.BoundsFromParent(BoundsWithOffset(_bottomTexture.Size(), offset)),
				Color.White
			);

			//#if DEBUG
			//			spriteBatch.Draw(
			//				Main.magicPixel,
			//				GetOuterDimensions().ToRectangle(),
			//				Color.Blue * 0.4f
			//			);

			//			spriteBatch.Draw(
			//				Main.magicPixel,
			//				TabFrame.GetOuterDimensions().ToRectangle(),
			//				Color.Orange * 0.4f
			//			);
			//#endif
		}
	}
}
