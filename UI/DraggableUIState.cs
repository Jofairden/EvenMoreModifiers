using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI
{
	/// <summary>
	/// A UIState which primarily goal is to provide dragging on click functionality to the main panel
	/// The drag panel MUST become assigned in OnInitialize
	/// Which means the base panel must be created, and then assigned, before calling the base OnInitialize
	/// </summary>
	public abstract class DraggableUIState : VisibilityUI
	{
		private bool _initialized;
		private UIPanel _dragPanel;
		private Vector2 _offset;
		private bool _dragging;

		protected void AssignDragPanel(UIPanel panel)
		{
			if (!_initialized) _dragPanel = panel;
		}

		public override void OnInitialize()
		{
			if (_dragPanel == null) throw new Exception("dragPanel for DraggableUIState is not set");
			_dragPanel.OnMouseDown += DragStart;
			_dragPanel.OnMouseUp += DragEnd;
			_initialized = true;
		}

		private void DragEnd(UIMouseEvent evt, UIElement listeningelement)
		{
			Vector2 end = evt.MousePosition;
			_dragging = false;

			_dragPanel.Left.Set(end.X - _offset.X, 0f);
			_dragPanel.Top.Set(end.Y - _offset.Y, 0f);

			Recalculate();
		}

		private void DragStart(UIMouseEvent evt, UIElement listeningelement)
		{
			//Should trigger on hit
			_offset = new Vector2(evt.MousePosition.X - _dragPanel.Left.Pixels, evt.MousePosition.Y - _dragPanel.Top.Pixels);
			_dragging = true;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Vector2 mousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
			if (_dragPanel.ContainsPoint(mousePosition))
			{
				Main.LocalPlayer.mouseInterface = true;
			}

			if (_dragging)
			{
				_dragPanel.Left.Set(mousePosition.X - _offset.X, 0f);
				_dragPanel.Top.Set(mousePosition.Y - _offset.Y, 0f);
				Recalculate();
			}
		}
	}
}