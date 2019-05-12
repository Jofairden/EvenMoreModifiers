using Loot.UI.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI.Core
{
	// @todo make toggable to 'keep' rolled modifier
	public class UIModifierPanel : UIElement
	{
		//private Modifier rolledModifier;

		private Texture2D _texture;
		private UIText _text;
		private const string DEFAULT_TEXT = "...";

		public override void OnInitialize()
		{
			_texture = Loot.Instance.GetTexture("UI/Common/GuiModifierPanel");
			_text = new UIText(DEFAULT_TEXT, 0.75f);
			Width.Set(_texture.Width, 0);
			Height.Set(_texture.Height, 0);
			Append(_text);
		}

		public void UpdateText(string line)
		{
			_text?.SetText(line);
		}

		public void ResetText()
		{
			_text?.SetText(DEFAULT_TEXT);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var parentBounds = GetOuterDimensions().ToRectangle();

			spriteBatch.Draw(
				_texture,
				_texture.BoundsFromParent(parentBounds),
				Color.White
			);

			if (base.IsMouseHovering)
			{
				Main.hoverItemName = _text.Text;
			}
		}
	}
}
