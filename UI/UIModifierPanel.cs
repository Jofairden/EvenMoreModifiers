using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Loot.UI
{
	// @todo make toggable to 'keep' rolled modifier
	public class UIModifierPanel : UIPanel
	{
		//private Modifier rolledModifier;
		
		private readonly UIText _text;
		private readonly string defaultText = "...";

		public UIModifierPanel() : base()
		{
			_text = new UIText("...", 0.75f);
			base.Append(_text);
		}

		public void UpdateText(string line)
		{
			_text?.SetText(line);
		}

		public void ResetText()
		{
			_text?.SetText(defaultText);
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