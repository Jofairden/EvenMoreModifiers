using Loot.Api.Ext;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.UI.Common.Controls.Panel
{
	internal class GuiPanel : GuiFramedElement
	{
		private readonly Texture2D _texture;

		public GuiPanel() : base(new Vector2(316, 50), new Vector2(10, 10))
		{
			_texture = Loot.Instance.GetTexture("UI/Common/Controls/Panel/GuiPanel");
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
