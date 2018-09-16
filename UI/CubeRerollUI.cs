using Loot.Core;
using Loot.Core.Cubes;
using Loot.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI
{
	/// <summary>
	/// A UIState that provides the ability to reroll an item's modifier
	/// </summary>
	public sealed class CubeRerollUI : CubeUI
	{
		private UIPanel _backPanel;
		internal UICubeItemPanel _cubePanel;
		internal UIRerollItemPanel _rerollItemPanel;
		internal ItemRollProperties _itemRollProperties;
		private UIModifierPanel[] _modifierPanels;
		private UIImageButton _rerollButton;
		private const float padding = 5f;

		public override void ToggleUI(UserInterface theInterface, UIState uiStateInstance)
		{
			base.ToggleUI(theInterface, uiStateInstance);

			// If ui closed, we need to retrieve the slotted items
			if (!Visible)
			{
				var ui = Loot.Instance.CubeRerollUI;

				SoundHelper.PlayCustomSound(SoundHelper.SoundType.CloseUI);

				// Clear of the slotted cube type
				ui._cubePanel?.item.TurnToAir();

				// If there is an item slotted
				if (ui._rerollItemPanel != null
					&& !ui._rerollItemPanel.item.IsAir)
				{
					Main.LocalPlayer.QuickSpawnClonedItem(ui._rerollItemPanel.item, ui._rerollItemPanel.item.stack);
					ui._rerollItemPanel.item.TurnToAir();
				}

				// Clear lines
				ui.UpdateModifierLines();
			}
		}

		public override bool IsItemValidForUISlot(Item item)
		{
			return _rerollItemPanel != null && _rerollItemPanel.CanTakeItem(item);
		}

		public override bool IsSlottedItemInCubeUI()
		{
			return _rerollItemPanel != null && !_rerollItemPanel.item.IsAir && EMMItem.GetItemInfo(_rerollItemPanel.item).SlottedInCubeUI;
		}

		public override Item SlottedItem => _rerollItemPanel.item;

		public override void OnInitialize()
		{
			_itemRollProperties = new ItemRollProperties();

			// Makes back panel, and assigns it as the drag panel
			_backPanel = new UIPanel();
			_backPanel.Width.Set(600f, 0f);
			_backPanel.Height.Set(200f, 0f);
			_backPanel.Left.Set(Main.screenWidth / 2f - _backPanel.Width.Pixels / 2f, 0f);
			_backPanel.Top.Set(Main.screenHeight / 2f - _backPanel.Height.Pixels / 2f, 0f);
			_backPanel.BackgroundColor = new Color(73, 94, 171);
			base.Append(_backPanel);
			AssignDragPanel(_backPanel);
			base.OnInitialize();

			Texture2D btnCloseTexture = ModLoader.GetTexture("Terraria/UI/InfoIcon_8");
			UIImageButton closeButton = new UIImageButton(btnCloseTexture);
			closeButton.Left.Set(_backPanel.Width.Pixels - btnCloseTexture.Width * 2f - padding, 0f);
			closeButton.Top.Set(0f, 0f);
			closeButton.Width.Set(btnCloseTexture.Width, 0f);
			closeButton.Height.Set(btnCloseTexture.Height, 0f);
			closeButton.OnClick += (evt, element) => { ToggleUI(Loot.Instance.CubeInterface, Loot.Instance.CubeRerollUI); };
			_backPanel.Append(closeButton);

			_cubePanel = new UICubeItemPanel(hintTexture: Loot.Instance.GetTexture("Core/Cubes/MagicalCube"), hintText: "Place a cube here");
			_cubePanel.Top.Set(0f, 0f);
			_backPanel.Append(_cubePanel);

			_rerollItemPanel = new UIRerollItemPanel(hintTexture: ModLoader.GetTexture("Terraria/Item_24"), hintText: "Place an item to reroll here");
			_rerollItemPanel.Top.Set(_cubePanel.Top.Pixels + _rerollItemPanel.Height.Pixels + padding, 0f);
			_backPanel.Append(_rerollItemPanel);

			_modifierPanels = new UIModifierPanel[4];
			for (int i = 0; i < 4; i++)
			{
				_modifierPanels[i] = new UIModifierPanel();
				var panel = _modifierPanels[i];

				panel.Left.Set(padding + _cubePanel.Left.Pixels + _cubePanel.Width.Pixels, 0f);
				panel.Height.Set(40, 0f);
				panel.Top.Set(i * padding + panel.Height.Pixels * i, 0f);
				panel.Width.Set(_backPanel.Width.Pixels - closeButton.Width.Pixels * 2f - panel.Left.Pixels - padding * 2f, 0f);
				panel.BackgroundColor = new Color(73, 94, 171);
				_backPanel.Append(panel);
			}

			Texture2D rerollTexture = ModLoader.GetTexture("Terraria/UI/Craft_Toggle_3");
			_rerollButton = new UIImageButton(rerollTexture);
			_rerollButton.Width.Set(UIItemPanel.panelwidth, 0f);
			_rerollButton.Height.Set(UIItemPanel.panelwidth, 0f);
			_rerollButton.Top.Set(_rerollItemPanel.Top.Pixels + _rerollItemPanel.Height.Pixels + padding, 0f);
			_rerollButton.OnClick += OnRerollClick;
			_backPanel.Append(_rerollButton);
		}

		private bool MatchesRerollRequirements()
		{
			return _rerollItemPanel != null
				   && _cubePanel != null
				   && !_rerollItemPanel.item.IsAir && !_cubePanel.item.IsAir
				   && _rerollItemPanel.CanTakeItem(_rerollItemPanel.item)
				   && _cubePanel.CanTakeItem(_cubePanel.item)
				   && !EMMItem.GetItemInfo(_rerollItemPanel.item).SealedModifiers;
		}

		private void RerollModifierPool(Item newItem)
		{
			// Roll new pool
			// @todo different rolling options
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnCubeReroll,
				Item = newItem,
				Player = Main.LocalPlayer,
				CustomData = new Dictionary<string, object>
				{
					{"Source", "CubeRerollUI"}
				}
			};

			var pool = EMMItem.GetItemInfo(newItem).RollNewPool(ctx, _itemRollProperties);
			pool?.ApplyModifiers(newItem);
			_rerollItemPanel.item = newItem.Clone();
		}

		private void OnRerollClick(UIMouseEvent evt, UIElement listeningelement)
		{
			if (MatchesRerollRequirements())
			{
				_itemRollProperties = new ItemRollProperties();
				_cubePanel.InteractionLogic(_itemRollProperties);
				_cubePanel.RecalculateStack();

				// No slotted cubes available
				if (_cubePanel.item.stack <= 0)
				{
					SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
					_cubePanel.item.TurnToAir();
					return;
				}

				// Refresh item
				Item newItem = new Item();
				newItem.netDefaults(_rerollItemPanel.item.type);
				// Clone to preserve modded data
				newItem = newItem.CloneWithModdedDataFrom(_rerollItemPanel.item);
				EMMItem.GetItemInfo(newItem).ModifierPool = null; // unload previous pool

				// Restore prefix
				if (_rerollItemPanel.item.prefix > 0)
				{
					newItem.Prefix(_rerollItemPanel.item.prefix);
				}

				RerollModifierPool(newItem);
				UpdateModifierLines();
				Main.PlaySound(SoundID.Item37, -1, -1);

				// Remove stack from player's inventory
				foreach (Item item in Main.LocalPlayer.inventory)
				{
					if (item.type == _cubePanel.item.type)
					{
						item.stack--;
						break;
					}
				}

				_cubePanel.RecalculateStack();

				// No slotted cubes available
				if (_cubePanel.item.stack <= 0)
				{
					_cubePanel.item.TurnToAir();
				}
			}
			else
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
			}
		}

		public void UpdateModifierLines()
		{
			foreach (var panel in _modifierPanels)
			{
				panel.ResetText();
			}

			if (_rerollItemPanel != null
				&& !_rerollItemPanel.item.IsAir
				&& _modifierPanels != null)
			{
				int i = 0;
				foreach (var lines in EMMItem.GetActivePool(_rerollItemPanel.item).Select(x => x.TooltipLines).Take(4))
				{
					string line = lines.Aggregate("", (current, tooltipLine) => current + $"{tooltipLine.Text} ");
					line = line.TrimEnd();
					_modifierPanels[i].UpdateText(line);
					i++;
				}

				Recalculate();
			}
		}
	}
}