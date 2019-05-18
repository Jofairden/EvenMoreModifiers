using Microsoft.Xna.Framework;
using Terraria.UI;

namespace Loot.UI
{
	internal abstract class GuiFramedElement : UIElement
	{
		protected UIElement Frame;

		protected CalculatedStyle FrameBounds => Frame.GetOuterDimensions();
		protected Vector2 FrameCenter => FrameBounds.Center();
		protected Vector2 FramePosition => FrameBounds.Position();

		protected GuiFramedElement(Vector2 size, Vector2 offset)
		{
			Width.Set(size.X, 0);
			Height.Set(size.Y, 0);
			Frame = new UIElement();
			Frame.Top.Set(offset.X, 0);
			Frame.Left.Set(offset.Y, 0);
			Frame.Width.Set(size.X - offset.X * 2, 0);
			Frame.Height.Set(size.Y - offset.Y * 2, 0);
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			Append(Frame);
			Recalculate();
		}

		//		protected override void DrawSelf(SpriteBatch spriteBatch)
		//		{
		//			base.DrawSelf(spriteBatch);
		//#if DEBUG
		//			spriteBatch.Draw(
		//				Main.magicPixel,
		//				GetOuterDimensions().ToRectangle(),
		//				Color.Red * 0.4f
		//			);
		//#endif
		//		}
	}
}
