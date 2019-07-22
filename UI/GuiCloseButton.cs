using Loot.Attributes;
using Loot.Ext;
using Loot.UI.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI
{
	internal class GuiCloseButton : UIElement
	{
		public override void OnInitialize()
		{
			Width.Set(Assets.Textures.GUI.CloseButtonTexture.Width, 0);
			Height.Set(Assets.Textures.GUI.CloseButtonTexture.Height, 0);
			Top.Set(20 + Assets.Textures.GUI.CloseButtonChainsTexture.Height, 0);
			Left.Set(422, 0);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var parentBounds = GetOuterDimensions().ToRectangle();
			Rectangle BoundsWithOffset(Vector2 bounds, Point off)
			{
				return new Rectangle(parentBounds.X + off.X, parentBounds.Y + off.Y, (int)bounds.X, (int)bounds.Y);
			}

			var _btn = Assets.Textures.GUI.CloseButtonTexture;
			spriteBatch.Draw(
				_btn,
				parentBounds,
				Color.White
			);

			var _chains = Assets.Textures.GUI.CloseButtonChainsTexture;
			Point offset = new Point(
				0,
				-_chains.Height
			);

			Rectangle rectangle = _chains.BoundsFromParent(BoundsWithOffset(_chains.Size(), offset));
			spriteBatch.Draw(
				_chains,
				rectangle,
				Color.White
			);

			if (parentBounds.Contains(Main.MouseScreen.ToPoint()))
			{
				Main.hoverItemName = "Close";
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					Loot.Instance.GuiState.ToggleUI(Loot.Instance.GuiInterface);
				}
			}
		}
	}
}
