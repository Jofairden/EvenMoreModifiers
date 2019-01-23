using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Loot.UI.Core
{
	// @todo make toggable to 'keep' rolled modifier
	public class UIModifierPanel : UIPanel
	{
		//private Modifier rolledModifier;

		private readonly UIText _text;
		private const string DEFAULT_TEXT = "...";

		public UIModifierPanel() : base()
		{
			_text = new UIText(DEFAULT_TEXT, 0.75f);
			base.Append(_text);
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
			base.DrawSelf(spriteBatch);

			if (base.IsMouseHovering)
			{
				Main.hoverItemName = _text.Text;
			}
		}
	}
}
