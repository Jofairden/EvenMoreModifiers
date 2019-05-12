using Microsoft.Xna.Framework;
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

		public new float VisibilityActive { get; private set; } = 1f;
		public new float VisibilityInactive { get; private set; } = 0.4f;
		public float VisiblityMultiplier => IsActive ? VisibilityActive : VisibilityInactive;
		public bool IsActive { get; private set; }

		public GuiTabToggle(GuiTabState state)
		{
			TargetState = state;
			_texture = GetTexture();
			OnClick += (evt, element) =>
			{
				WhenClicked?.Invoke(evt, element, this);
			};
		}

		public override void OnInitialize()
		{
			Height.Set(_texture.Height, 0);
			Width.Set(_texture.Width, 0);
			_button = new UIImageButton(_texture);
			Append(_button);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			spriteBatch.Draw(_texture, dimensions.Position(), Color.White * VisiblityMultiplier);
		}

		private Texture2D GetTexture()
		{
			switch (TargetState)
			{
				default:
				case GuiTabState.CUBING:
					return Loot.Instance.GetTexture("UI/Common/GuiTabCubing");
				case GuiTabState.ESSENCE:
					return Loot.Instance.GetTexture("UI/Common/GuiTabEssencecraft");
				case GuiTabState.SOULFORGE:
					return Loot.Instance.GetTexture("UI/Common/GuiTabSoulforge");
			}
		}

		public void SetVisibility(float whenActive, float whenInactive)
		{
			VisibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
			VisibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
			_button.SetVisibility(whenActive, whenInactive);
		}

		public void SetActive(bool state)
		{
			IsActive = state;
		}
	}
}
