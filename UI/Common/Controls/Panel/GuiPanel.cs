using Loot.Ext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.UI.Common.Controls.Panel
{
	internal class GuiPanel : GuiFramedElement
	{
		private static Texture2D _texture => Assets.Textures.GUI.PanelTexture;

		public GuiPanel() : base(new Vector2(316, 50), new Vector2(10, 10))
		{
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			var parentBounds = GetOuterDimensions().ToRectangle();

			spriteBatch.Draw(
				_texture,
				_texture.BoundsFromParent(parentBounds),
				Color.White
			);

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
