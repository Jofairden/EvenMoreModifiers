using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Loot.Api.Ext
{
	internal static class GuiUtils
	{
		internal const int SPACING = 25;
		internal const int PADDING = 5;

		public static Rectangle BoundsFromParent(this Texture2D texture, Rectangle parentBounds)
		{
			var texBounds = texture.Bounds;
			return new Rectangle(parentBounds.X, parentBounds.Y, texBounds.Width, texBounds.Height);
		}

		public static T RightOf<T>(this T @this, UIElement @that) where T : UIElement
		{
			@this.Left.Set(@that.Left.Pixels + @that.Width.Pixels + PADDING, 0f);
			return @this;
		}

		public static T LeftOf<T>(this T @this, UIElement @that) where T : UIElement
		{
			@this.Left.Set(@that.Left.Pixels - @that.Width.Pixels - PADDING, 0f);
			return @this;
		}

		public static T Below<T>(this T @this, UIElement @that) where T : UIElement
		{
			@this.Top.Set(@that.Top.Pixels + @that.Height.Pixels + PADDING, 0f);
			return @this;
		}

		public static T Above<T>(this T @this, UIElement @that) where T : UIElement
		{
			@this.Top.Set(@that.Top.Pixels - @that.Height.Pixels - PADDING, 0f);
			return @this;
		}
	}
}
