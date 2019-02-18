using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI.Common.Controls.Button
{
	internal class GuiImageButton : GuiButton
	{
		public UIImageButton Button { get; }

		public delegate void WhenClickedEvent(UIMouseEvent evt, UIElement listeningElement, UIImageButton btn);
		public event WhenClickedEvent WhenClicked;

		internal GuiImageButton(ButtonType buttonType, Texture2D texture) : base(buttonType)
		{
			Button = new UIImageButton(texture);
			Button.Width.Set(Width.Pixels, 0);
			Button.Height.Set(Height.Pixels, 0);
			Button.OnClick += (evt, element) => WhenClicked?.Invoke(evt, element, Button);
			Frame.Append(Button);
		}

		//protected override void DrawSelf(SpriteBatch spriteBatch)
		//{
		//	base.DrawSelf(spriteBatch);
		//	//#if DEBUG
		//	//			spriteBatch.Draw(
		//	//				Main.magicPixel,
		//	//				FrameBounds.ToRectangle(),
		//	//				Color.Orange * 0.4f
		//	//			);
		//	//#endif

		//	//#if DEBUG
		//	//			spriteBatch.Draw(
		//	//				Main.magicPixel,
		//	//				Button.GetOuterDimensions().ToRectangle(),
		//	//				Color.Blue * 0.4f
		//	//			);
		//	//#endif
		//}
	}
}
