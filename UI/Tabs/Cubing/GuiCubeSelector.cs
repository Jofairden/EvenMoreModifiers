using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Api.Cubes;
using Loot.Api.Ext;
using Loot.Ext;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Common.Controls.Panel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Loot.UI.Tabs.Cubing
{
	/// <summary>
	/// The cube selector is a panel below the cubing UI used to easily insert a cube into the UI
	/// </summary>
	internal class GuiCubeSelector : GuiPanel
	{
		private const string NO_CUBES_FOUND = "No cubes found in inventory";
		private const int CUBES_PER_PAGE = 5;

		private readonly List<UIElement> _cubes = new List<UIElement>();
		private readonly GuiArrowButton _arrowLeft;
		private readonly GuiArrowButton _arrowRight;

		private int _maxOffset;
		private int _currentOffset;

		public GuiCubeSelector()
		{
			_arrowLeft = new GuiArrowButton(GuiArrowButton.ArrowDirection.LEFT) { HoverText = "Previous" };
			_arrowRight = new GuiArrowButton(GuiArrowButton.ArrowDirection.RIGHT) { HoverText = "Next" };
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			_arrowLeft.Activate();
			_arrowLeft.Left.Set(-_arrowLeft.Width.Pixels, 0);
			_arrowLeft.WhenClicked += delegate (UIMouseEvent evt, UIElement element, GuiArrowButton btn)
			{
				_currentOffset--;
				btn.CanBeClicked = _maxOffset > 0 && _currentOffset > 0;
				_arrowRight.CanBeClicked = _maxOffset > 0 && _currentOffset < _maxOffset;
				DetermineAvailableCubes();
			};
			Append(_arrowLeft);

			_arrowRight.Activate();
			_arrowRight.Left.Set(Width.Pixels, 0);
			_arrowRight.WhenClicked += delegate (UIMouseEvent evt, UIElement element, GuiArrowButton btn)
			{
				_currentOffset++;
				_arrowLeft.CanBeClicked = _maxOffset > 0 && _currentOffset > 0;
				btn.CanBeClicked = _maxOffset > 0 && _currentOffset < _maxOffset;
				DetermineAvailableCubes();
			};
			Append(_arrowRight);
		}

		private GuiItemButton _lastSelected;

		// TODO select/deselect not working? (scale/color??)
		public void DeselectPreviousCube()
		{
			if (_lastSelected != null)
			{
				_lastSelected.DrawScale = 0.75f;
				_lastSelected.DynamicScaling = true;
				_lastSelected.DrawColor = null;
				_lastSelected = null;
			}
		}

		private void SetSelectedCube(UIElement element)
		{
			if (element != null && element is GuiItemButton itemButton)
			{
				DeselectPreviousCube();
				itemButton.DrawScale = 0.88f;
				itemButton.DynamicScaling = false;
				itemButton.DrawColor = Color.White;
				_lastSelected = itemButton;
			}
		}

		public void SetSelectedCubeByType(int type)
		{
			SetSelectedCube(
				_cubes.FirstOrDefault(x => x is GuiItemButton itemButton
										   && itemButton.Item.type == type)
			);
		}

		public void DetermineAvailableCubes()
		{
			_cubes.Clear();

			var foundItems =
				Main.LocalPlayer.inventory.GetDistinctModItems<RerollingCube>()
					.Select(i => (name: i.item.Name, i.item.type, stack: Main.LocalPlayer.inventory.CountItemStack(i.item.type, true)));

			var foundCount = foundItems.Count();

			if (_currentOffset > 0)
			{
				foundItems = foundItems.Skip(_currentOffset * CUBES_PER_PAGE);
			}

			var items = foundItems.Take(CUBES_PER_PAGE).ToList();


			_maxOffset = (int)Math.Floor(foundCount / (CUBES_PER_PAGE + 1f));
			_arrowLeft.CanBeClicked = _maxOffset > 0 && _currentOffset > 0;
			_arrowRight.CanBeClicked = _maxOffset > 0 && _currentOffset < _maxOffset;


			for (int i = 0; i < items.Count; i++)
			{
				var (name, type, stack) = items[i];
				var button = new GuiItemButton(GuiButton.ButtonType.StoneOuterBevel, type, stack, hintOnHover: $"Click to use {name}")
				{
					Left = new StyleDimension(15f, 0f),
					DrawBackground = false,
					DrawScale = 0.75f,
					DrawStack = true,
					ShowOnlyHintOnHover = true
				};
				button.OnMouseOver += (evt, element) => { Main.PlaySound(12); };
				button.OnClick += (evt, element) =>
				{
					OnCubeClick(type);
					SetSelectedCube(element);
				};
				_cubes.Add(button);
			}

			UpdateCubeFrame();
		}

		private void UpdateCubeFrame()
		{
			Frame.RemoveAllChildren();
			if (!_cubes.Any())
			{
				Frame.Append(new UIText(NO_CUBES_FOUND));
			}
			else
			{
				// TODO
				//var rememberedSelection = _lastSelected;
				//DeselectPreviousCube();
				//SetSelectedCube(rememberedSelection);

				int i = 0;
				var elementSet = _cubes.ToList();
				foreach (var element in elementSet)
				{
					if (i > 0)
					{
						element.RightOf(elementSet[i - 1]);
					}
					Frame.Append(element);
					i++;
				}
			}
		}

		private void OnCubeClick(int type)
		{
			if (Loot.Instance.GuiState.GetCurrentTab() is GuiCubingTab cubingTab)
			{
				cubingTab._cubeButton.ChangeItem(type);
			}
		}
	}
}
