using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI.Common
{
	internal class GuiTabToggle : UIElement
	{
		public readonly GuiTabState TargetState;

		public delegate void WhenClickedEvent(UIMouseEvent evt, UIElement listeningElement, GuiTabToggle toggle);
		public event WhenClickedEvent WhenClicked;

		private UIImageButton _button;
		private Texture2D _texture;

		public GuiTabToggle(GuiTabState state)
		{
			_texture = Loot.Instance.GetTexture("UI/Common/GuiTabToggle");
			TargetState = state;
			OnClick += (evt, element) => { WhenClicked?.Invoke(evt, element, this); };
		}

		public override void OnInitialize()
		{
			Height.Set(_texture.Height, 0);
			Width.Set(_texture.Width, 0);
			_button = new UIImageButton(_texture);
			Append(_button);
		}
	}
}
