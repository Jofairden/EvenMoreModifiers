using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common.Core
{
	/// <summary>
	/// A UIState which primarily goal is to provide dragging on click functionality to another element (or itself)
	/// </summary>
	public abstract class DraggableUIState : VisibilityUI
	{
		private int _dragging;

		private readonly List<(UIElement, UIElement)> _draggingList = new List<(UIElement, UIElement)>();
		private readonly Dictionary<UIElement, Vector2> _offsets = new Dictionary<UIElement, Vector2>();

		protected void AddDragging(UIElement dragger, UIElement draggable = null)
		{
			draggable = draggable ?? dragger;
			_draggingList.Add((dragger, draggable));
			if (!_offsets.ContainsKey(dragger))
			{
				_offsets.Add(dragger, Vector2.Zero);
			}

			dragger.OnMouseDown += (evt, element) =>
			{
				//start
				_offsets[dragger] = new Vector2(evt.MousePosition.X - draggable.Left.Pixels, evt.MousePosition.Y - draggable.Top.Pixels);
				_dragging++;
			};

			dragger.OnMouseUp += (evt, element) =>
			{
				//end
				Vector2 end = evt.MousePosition;
				_dragging--;

				draggable.Left.Set(end.X - _offsets[dragger].X, 0f);
				draggable.Top.Set(end.Y - _offsets[dragger].Y, 0f);

				dragger.Recalculate();
				draggable.Recalculate();
			};
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Main.LocalPlayer.mouseInterface = IsMouseHovering;

			if (_dragging <= 0)
			{
				return;
			}

			foreach (var (dragger, draggable) in _draggingList)
			{
				draggable.Left.Set(Main.MouseScreen.X - _offsets[dragger].X, 0f);
				draggable.Top.Set(Main.MouseScreen.Y - _offsets[dragger].Y, 0f);

				dragger.Recalculate();
				draggable.Recalculate();
			}
		}
	}
}
