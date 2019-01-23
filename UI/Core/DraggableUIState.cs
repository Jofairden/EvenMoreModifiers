using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI.Core
{
	/// <summary>
	/// A UIState which primarily goal is to provide dragging on click functionality to the main panel
	/// The drag panel MUST become assigned in OnInitialize
	/// Which means the base panel must be created, and then assigned, before calling the base OnInitialize
	/// </summary>
	public abstract class DraggableUIState : VisibilityUI
	{
		private bool _initialized;
		private List<UIPanel> _dragPanels = new List<UIPanel>();
		private List<Vector2> _offsets = new List<Vector2>();
		private bool _dragging;

		protected void AddDragPanel(UIPanel panel)
		{
			if (_initialized) return;
			_dragPanels.Add(panel);
			_offsets.Add(new Vector2());
		}

		public override void OnInitialize()
		{
			if (_dragPanels == null)
			{
				Log4c.Logger.Warn("_dragPanels was uninitialized in DraggableUIState");
				_dragPanels = new List<UIPanel>();
			}

			if (_offsets == null)
			{
				Log4c.Logger.Warn("_offsets was uninitialized in DraggableUIState");
				_offsets = new List<Vector2>();
			}

			foreach (var panel in _dragPanels)
			{
				panel.OnMouseDown += DragStart;
				panel.OnMouseUp += DragEnd;
			}

			_initialized = true;
		}

		private void DragEnd(UIMouseEvent evt, UIElement _)
		{
			Vector2 end = evt.MousePosition;
			_dragging = false;

			for (int i = 0; i < _dragPanels.Count; i++)
			{
				var panel = _dragPanels[i];
				var offset = _offsets[i];
				panel.Left.Set(end.X - offset.X, 0f);
				panel.Top.Set(end.Y - offset.Y, 0f);
			}

			Recalculate();
		}

		private void DragStart(UIMouseEvent evt, UIElement _)
		{
			//Should trigger on hit

			for (int i = 0; i < _dragPanels.Count; i++)
			{
				var panel = _dragPanels[i];
				_offsets[i] = new Vector2(evt.MousePosition.X - panel.Left.Pixels, evt.MousePosition.Y - panel.Top.Pixels);
			}

			_dragging = true;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Vector2 mousePosition = new Vector2((float) Main.mouseX, (float) Main.mouseY);

			for (int i = 0; i < _dragPanels.Count; i++)
			{
				var panel = _dragPanels[i];
				var offset = _offsets[i];

				if (!Main.LocalPlayer.mouseInterface
				    && panel.ContainsPoint(mousePosition))
				{
					Main.LocalPlayer.mouseInterface = true;
				}

				if (_dragging)
				{
					panel.Left.Set(mousePosition.X - offset.X, 0f);
					panel.Top.Set(mousePosition.Y - offset.Y, 0f);
					Recalculate();
				}
			}
		}
	}
}
