using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Loot.UI.Common.Controls.Panel
{
	internal class GuiTextPanel : GuiPanel
	{
		private const string DEFAULT_TEXT = "...";
		private string _hoverText;
		private UIText _text;

		public override void OnInitialize()
		{
			base.OnInitialize();
			_text = new UIText(DEFAULT_TEXT, 0.75f);
			Frame.Append(_text);
		}

		public void SetHoverText(string line)
		{
			_hoverText = line;
		}

		public void UpdateText(string line)
		{
			_text?.SetText(line);
			_text?.Top.Set(Main.fontMouseText.MeasureString(line).Y * 0.375f, 0);
		}

		public void ResetText()
		{
			_text?.SetText(DEFAULT_TEXT);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering)
			{
				Main.hoverItemName = _hoverText ?? _text.Text;
			}
		}
	}
}
