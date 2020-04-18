using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Cubes;
using Loot.Api.Ext;
using Loot.Api.Mechanism;
using Loot.Api.Strategy;
using Loot.Cubes;
using Loot.Essences;
using Loot.Ext;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Common.Controls.Panel;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI.Tabs.CraftingTab
{
	internal class CraftingComponentSelector<T> : GuiPanel where T : ModItem
	{
		public virtual string NotFoundString() => "No components found in inventory";
		public virtual int ComponentsPerPage() => 5;

		private readonly List<UIElement> _components = new List<UIElement>();
		private readonly GuiArrowButton _arrowLeft;
		private readonly GuiArrowButton _arrowRight;
		private GuiItemButton _lastSelected;

		internal List<CraftingComponentLink> ComponentLinks = new List<CraftingComponentLink>();
		internal Func<Item, IEnumerable<CraftingComponentLink>> FindComponents;
		internal Func<CraftingComponentLink, Item, bool> VerifyComponent;
		internal Func<CraftingComponentLink, bool> OnComponentClick;

		private int _maxOffset;
		private int _currentOffset;

		public CraftingComponentSelector()
		{
			_arrowLeft = new GuiArrowButton(GuiArrowButton.ArrowDirection.LEFT) { HoverText = "Previous" };
			_arrowRight = new GuiArrowButton(GuiArrowButton.ArrowDirection.RIGHT) { HoverText = "Next" };
			OnComponentClick += _OnComponentClick;
			FindComponents += _FindComponents;
			VerifyComponent += _VerifyComponent;
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
				CalculateAvailableComponents();
			};
			Append(_arrowLeft);

			_arrowRight.Activate();
			_arrowRight.Left.Set(Width.Pixels, 0);
			_arrowRight.WhenClicked += delegate (UIMouseEvent evt, UIElement element, GuiArrowButton btn)
			{
				_currentOffset++;
				_arrowLeft.CanBeClicked = _maxOffset > 0 && _currentOffset > 0;
				btn.CanBeClicked = _maxOffset > 0 && _currentOffset < _maxOffset;
				CalculateAvailableComponents();
			};
			Append(_arrowRight);

			HAlign = 0.5f;
			VAlign = 1f;
		}

		private bool _OnComponentClick(CraftingComponentLink link)
		{
			// When a component is clicked, we need to re-verify that the link to it still exists and is still valid
			CalculateAvailableComponents();
			return link?.Component.modItem is MagicalCube && link.Component.stack > 0;
		}

		private bool _VerifyComponent(CraftingComponentLink link, Item item)
		{
			var props = new RollingStrategyProperties();
			if (!(link.Component.modItem is T))
			{
				return false;
			}

			var component = link.Component.modItem;
			RollingStrategy strategy = null;

			if (component is CubeOfSealing)
			{
				return true;
			}
			if (component is RerollingCube cube)
			{
				strategy = cube.GetRollingStrategy(item, props);
			}
			else if (component is EssenceItem essence)
			{
				strategy = essence.GetRollingStrategy(item, props);
			}

			var ctx = GetContext(item);
			ctx.Strategy = strategy;
			var preRolledLines = strategy?.PreRoll(ModifierPoolMechanism.GetPool(ctx), ctx, props);
			return preRolledLines?.All(x => x.CanRoll(ctx)) ?? false;
		}

		private IEnumerable<CraftingComponentLink> _FindComponents(Item item)
		{
			return Main.LocalPlayer.inventory.GetDistinctModItems<T>()
				.Select(x =>
				{
					var component = x as ModItem;
					component.item.stack = Main.LocalPlayer.inventory.CountItemStack(component.item.type, true);
					return new CraftingComponentLink(component.item, CraftingComponentLink.ComponentSource.Inventory);
				});
		}

		public bool IsAvailable(int type)
		{
			return ComponentLinks.Any(x => x.Component.type == type);
		}

		public IEnumerable<CraftingComponentLink> GetComponentLinks(int type)
		{
			return ComponentLinks.Where(x => x.Component.type == type);
		}

		// TODO select/deselect not working? (scale/color??)
		public void DeselectPreviousComponent()
		{
			if (_lastSelected != null)
			{
				_lastSelected.DrawScale = 0.75f;
				_lastSelected.DynamicScaling = true;
				_lastSelected.DrawColor = null;
				_lastSelected = null;
			}
		}

		private void SetSelectedComponent(UIElement element)
		{
			if (element != null && element is GuiItemButton itemButton)
			{
				DeselectPreviousComponent();
				itemButton.DrawScale = 0.88f;
				itemButton.DynamicScaling = false;
				itemButton.DrawColor = Color.White;
				_lastSelected = itemButton;
			}
		}

		private ModifierContext GetContext(Item item)
		{
			return new ModifierContext
			{
				Item = item,
				Player = Main.LocalPlayer
			};
		}

		public void CalculateAvailableComponents(Item item = null)
		{
			_components.Clear();
			ComponentLinks.Clear();

			ComponentLinks = FindComponents?.GetInvocationList().SelectMany(x => (IEnumerable<CraftingComponentLink>)x.DynamicInvoke(item)).ToList() ?? new List<CraftingComponentLink>();

			if (item != null && !item.IsAir)
				ComponentLinks = ComponentLinks.Where(x => VerifyComponent?.GetInvocationList().Select(y => (bool)y.DynamicInvoke(x, item)).All(z => z) ?? false)
					.ToList();

			var selection = ComponentLinks.AsEnumerable();

			var foundCount = selection.Count();
			_maxOffset = (int)Math.Floor(foundCount / (ComponentsPerPage() + 1f));
			if (_maxOffset < _currentOffset) _currentOffset = _maxOffset;

			if (_currentOffset > 0)
			{
				selection = selection.Skip(_currentOffset * ComponentsPerPage());
			}

			var items = selection.Take(ComponentsPerPage()).ToList();

			_arrowLeft.CanBeClicked = _maxOffset > 0 && _currentOffset > 0;
			_arrowRight.CanBeClicked = _maxOffset > 0 && _currentOffset < _maxOffset;

			foreach (var link in items)
			{
				var name = link.Component.Name;
				var type = link.Component.type;
				var stack = link.Component.stack;

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
					if (OnComponentClick?.GetInvocationList().All(x => (bool)x.DynamicInvoke(link)) ?? false)
					{
						SetSelectedComponent(element);
					}

				};
				_components.Add(button);
			}

			UpdateComponentFrame();
		}

		private void UpdateComponentFrame()
		{
			Frame.RemoveAllChildren();
			if (!_components.Any())
			{
				Frame.Append(new UIText(NotFoundString()));
			}
			else
			{
				// TODO
				//var rememberedSelection = _lastSelected;
				//DeselectPreviousCube();
				//SetSelectedCube(rememberedSelection);

				int i = 0;
				var elementSet = _components.ToList();
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
	}
}
