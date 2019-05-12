
using Loot.Sounds;
using Loot.UI.Common.Core;
using Loot.UI.Common.Tabs.Cubing;
using Loot.UI.Common.Tabs.EssenceCrafting;
using Loot.UI.Common.Tabs.Soulforging;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.UI.Common
{
	// TODO make items in inventory not targetable when in this UI
	/// <summary>
	/// The GuiTabWindow is a window with tabs on the right side that are toggable
	/// The window will only display one tab's contents at a time
	/// </summary>
	internal class GuiTabWindow : DraggableUIState
	{
		private GuiHeader _header;
		private GuiCloseButton _closeButton;
		private GuiTab _currentTab;
		internal GuiTabState _currentTabState = GuiTabState.CUBING;

		internal readonly Dictionary<GuiTabState, GuiTab> _tabs = new Dictionary<GuiTabState, GuiTab>
		{
			{GuiTabState.CUBING, new GuiCubingTab()},
			{GuiTabState.ESSENCE, new GuiEssenceTab()},
			{GuiTabState.SOULFORGE, new GuiSoulforgeTab()},
		};

		internal readonly Dictionary<GuiTabState, GuiTabToggle> _toggles = new Dictionary<GuiTabState, GuiTabToggle>
		{
			{GuiTabState.CUBING, new GuiTabToggle(GuiTabState.CUBING)},
			{GuiTabState.ESSENCE, new GuiTabToggle(GuiTabState.ESSENCE)},
			{GuiTabState.SOULFORGE, new GuiTabToggle(GuiTabState.SOULFORGE)}
		};

		public GuiTab GetTab()
		{
			_tabs.TryGetValue(_currentTabState, out _currentTab);
			_currentTab?._GetPageHeight();
			return _currentTab;
		}

		public T GetTab<T>() where T : GuiTab
		{
			return (T)_tabs.Values.FirstOrDefault(t => t.GetType() == typeof(T));
		}

		private void ToggleTab(UIMouseEvent evt, UIElement element, GuiTabToggle toggle)
		{
			if (_currentTabState == toggle.TargetState)
			{
				return;
			}

			SoundHelper.PlayCustomSound(SoundHelper.SoundType.OpenUI);
			UpdateTabTo(toggle.TargetState);
		}

		public void UpdateTabTo(GuiTabState newState)
		{
			_toggles[newState].SetActive(true);
			_toggles[_currentTabState].SetActive(false);
			_currentTabState = _toggles[newState].TargetState;
			UpdateTab();
		}

		public void UpdateTab()
		{
			if (HasChild(_currentTab))
			{
				RemoveChild(_currentTab);
			}

			GetTab();
			_header.SetHeader(_currentTab.Header);
			Append(_currentTab);
			_currentTab.Activate();
			UpdateWindow();
		}

		public void UpdateWindow()
		{
			Height.Set(_header.Height.Pixels + _currentTab.TotalHeight, 0);
			Recalculate();
		}

		public void CenterWindow()
		{
			float useW = Main.screenWidth > Main.minScreenW ? Main.screenWidth : Main.minScreenW;
			float useH = Main.screenHeight > Main.minScreenH ? Main.screenHeight : Main.minScreenH;
			Left.Set(useW * 0.5f - Width.Pixels * 0.5f, 0);
			Top.Set(useH * 0.5f - Height.Pixels * 0.5f, 0);
		}

		public override void OnInitialize()
		{
			Width.Set(422, 0);

			_header = new GuiHeader();

			Append(_header);
			AddDragging(_header, this);

			_closeButton = new GuiCloseButton();
			Append(_closeButton);

			foreach (var kvp in _tabs)
			{
				kvp.Value.Top.Set(_header.GetOffset(), 0);
			}

			var togglePosition = new Vector2(422, _header.GetOffset());
			var toggleOffset = Vector2.UnitY * 10;

			foreach (var toggle in _toggles.Select(x => x.Value).Take(1)) // TODO remove Take(1) later
			{
				toggle.Activate();
				toggle.WhenClicked += ToggleTab;
				var usePosition = togglePosition + toggleOffset;
				toggle.Left.Set(usePosition.X, 0);
				toggle.Top.Set(usePosition.Y, 0);
				toggleOffset.Y += toggle.Height.Pixels;
				Append(toggle);
			}

			_toggles[_currentTabState].SetActive(true);
			UpdateTab();
			CenterWindow();
		}

		public override void ToggleUI(UserInterface theInterface, UIState uiStateInstance = null)
		{
			base.ToggleUI(theInterface, uiStateInstance);

			if (Visible)
			{
				UpdateTab();
			}
			_currentTab?.ToggleUI(Visible);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			if (IsMouseHovering || MouseOverUI())
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}

		private bool MouseOverUI()
		{
			var rect = new Rectangle((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels);
			return rect.Contains(Main.MouseScreen.ToPoint());
		}
	}
}
