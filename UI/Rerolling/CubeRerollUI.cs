using System.Collections.Generic;
using System.Linq;
using Loot.Core.Cubes;
using Loot.Core.System.Modifier;
using Loot.Ext;
using Loot.Sounds;
using Loot.UI.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI.Rerolling
{
	// @todo add horizontal panel blow base with list of cubes in inventory to use, click to put into UI slot feature

	/// <summary>
	/// A UIState that provides the ability to reroll an item's modifier
	/// </summary>
	public sealed class CubeRerollUI : CubeUI
	{
		private const float PADDING = 5f;

		internal UICubeItemPanel _cubePanel;
		internal UIRerollItemPanel _rerollItemPanel;
		internal ItemRollProperties _itemRollProperties;

		private UIPanel _backPanel;
		private UIImageButton _rerollButton;
		private UIPanel _cubeListPanel;
		private UIModifierPanel[] _modifierPanels;

		public override void ToggleUI(UserInterface theInterface, UIState uiStateInstance)
		{
			base.ToggleUI(theInterface, uiStateInstance);

			if (Visible)
			{
				DetermineAvailableCubes();
				return;
			}

			// If ui closed, we need to retrieve the slotted items
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

		internal void DetermineAvailableCubes()
		{
			_cubeListPanel.RemoveAllChildren();

			var items =
				Main.LocalPlayer.inventory.GetDistinctModItems<RerollingCube>()
					.Select(i => i.item.type);

			int current = 0;
			foreach (var type in items)
			{
				var selectorPanel = new UICubeSelectorPanel(type)
				{
					Left = new StyleDimension {Pixels = UIItemPanel.PANEL_WIDTH * current}
				};
				if (_cubePanel.item.type != type)
				{
					selectorPanel.DrawScale = 0.8f;
					selectorPanel.DrawColor = Color.LightGray * 0.5f;
				}

				selectorPanel.RecalculateStack();
				_cubeListPanel.Append(selectorPanel);
				current++;
			}

			if (current <= 0)
			{
				_cubeListPanel.Append(new UIText("No cubes found in inventory"));
			}

			_cubeListPanel.Recalculate();
		}

		public override bool IsItemValidForUISlot(Item item)
			=> _rerollItemPanel != null
			   && _rerollItemPanel.CanTakeItem(item);

		public override bool IsSlottedItemInCubeUI()
			=> _rerollItemPanel != null
			   && !_rerollItemPanel.item.IsAir
			   && EMMItem.GetItemInfo(_rerollItemPanel.item).SlottedInCubeUI;

		public override Item SlottedItem => _rerollItemPanel.item;

		// TODO refactor UI code a bit. 
		public override void OnInitialize()
		{
			_itemRollProperties = new ItemRollProperties();

			float halfScreenWidth = Main.screenWidth / 2f;
			float halfScreenHeight = Main.screenHeight / 2f;
			Color baseColor = new Color(73, 94, 171);

			_backPanel = new UIPanel();
			_backPanel.Width.Set(600f, 0f);
			_backPanel.Height.Set(200f, 0f);
			_backPanel.Left.Set(halfScreenWidth - _backPanel.Width.Pixels / 2f, 0f);
			_backPanel.Top.Set(halfScreenHeight - _backPanel.Height.Pixels / 2f, 0f);

			_cubeListPanel = new UIPanel
			{
				Width = new StyleDimension {Pixels = 600f},
				Height = new StyleDimension {Pixels = 50f},
				Left = new StyleDimension {Pixels = _backPanel.Left.Pixels},
				Top = new StyleDimension {Pixels = _backPanel.Top.Pixels + _backPanel.Height.Pixels},
			};

			Append(_cubeListPanel);
			Append(_backPanel);
			AddDragPanel(_cubeListPanel);
			AddDragPanel(_backPanel);
			base.OnInitialize();

			Texture2D btnCloseTexture = ModContent.GetTexture("Terraria/UI/InfoIcon_8");
			UIImageButton closeButton = new UIImageButton(btnCloseTexture);
			closeButton.Left.Set(_backPanel.Width.Pixels - btnCloseTexture.Width * 2f - PADDING, 0f);
			closeButton.Top.Set(0f, 0f);
			closeButton.Width.Set(btnCloseTexture.Width, 0f);
			closeButton.Height.Set(btnCloseTexture.Height, 0f);
			closeButton.OnClick += (evt, element) => { ToggleUI(Loot.Instance.CubeInterface, Loot.Instance.CubeRerollUI); };
			_backPanel.Append(closeButton);

			_cubePanel = new UICubeItemPanel(hintTexture: Loot.Instance.GetTexture("Core/Cubes/MagicalCube"), hintText: "Place a cube here");
			_cubePanel.Top.Set(0f, 0f);
			_backPanel.Append(_cubePanel);

			_rerollItemPanel = new UIRerollItemPanel(hintTexture: ModContent.GetTexture("Terraria/Item_24"), hintText: "Place an item to reroll here");
			_rerollItemPanel.Top.Set(_cubePanel.Top.Pixels + _rerollItemPanel.Height.Pixels + PADDING, 0f);
			_backPanel.Append(_rerollItemPanel);

			_modifierPanels = new UIModifierPanel[4];
			for (int i = 0; i < 4; i++)
			{
				_modifierPanels[i] = new UIModifierPanel();
				var panel = _modifierPanels[i];

				panel.Left.Set(PADDING + _cubePanel.Left.Pixels + _cubePanel.Width.Pixels, 0f);
				panel.Height.Set(40, 0f);
				panel.Top.Set(i * PADDING + panel.Height.Pixels * i, 0f);
				panel.Width.Set(_backPanel.Width.Pixels - closeButton.Width.Pixels * 2f - panel.Left.Pixels - PADDING * 2f, 0f);
				panel.BackgroundColor = baseColor;
				_backPanel.Append(panel);
			}

			Texture2D rerollTexture = ModContent.GetTexture("Terraria/UI/Craft_Toggle_3");
			_rerollButton = new UIImageButton(rerollTexture);
			_rerollButton.Width.Set(UIItemPanel.PANEL_WIDTH, 0f);
			_rerollButton.Height.Set(UIItemPanel.PANEL_WIDTH, 0f);
			_rerollButton.Top.Set(_rerollItemPanel.Top.Pixels + _rerollItemPanel.Height.Pixels + PADDING - _cubeListPanel.Height.Pixels, 0.33f);
			_rerollButton.Left.Set(_rerollItemPanel.Left.Pixels + PADDING * 2f, 0f);
			_rerollButton.OnClick += OnRerollClick;
			_backPanel.Append(_rerollButton);
		}

		private bool MatchesRerollRequirements()
		{
			bool match = _rerollItemPanel != null
			             && _cubePanel != null
			             && !_rerollItemPanel.item.IsAir && !_cubePanel.item.IsAir
			             && _rerollItemPanel.CanTakeItem(_rerollItemPanel.item)
			             && _cubePanel.CanTakeItem(_cubePanel.item)
			             && !EMMItem.GetItemInfo(_rerollItemPanel.item).SealedModifiers;

			bool hasItem = Main.LocalPlayer.inventory.Any(x => x.type == _cubePanel.item.type)
			               || (Main.mouseItem?.type == _cubePanel?.item?.type);

			return match && hasItem;
		}

		private void RerollModifierPool(Item newItem)
		{
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

				bool checkMouse = true;
				// Remove stack from player's inventory
				// .Take 58 because 59th slot is MouseItem for some reason.
				foreach (Item item in Main.LocalPlayer.inventory.Take(58))
				{
					if (item.type == _cubePanel.item.type)
					{
						checkMouse = false;
						item.stack--;
						break;
					}
				}

				if (checkMouse)
				{
					// This check should be redundant; otherwise we should never have reached this point
					//if (Main.mouseItem?.type == _cubePanel.item.type)
					Main.mouseItem.stack--;
					if (Main.mouseItem.stack <= 0)
					{
						Main.mouseItem.TurnToAir();
					}
				}

				_cubePanel.RecalculateStack();
				DetermineAvailableCubes();

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

			if (_rerollItemPanel == null
			    || _rerollItemPanel.item.IsAir
			    || _modifierPanels == null)
			{
				return;
			}

			int i = 0;

			IEnumerable<ModifierTooltipLine[]> GetTooltipLines(Item item)
				=> EMMItem.GetActivePool(item)
					.Select(x => x.GetTooltip().Build())
					.Take(4)
					.Where(x => x != null);

			foreach (var lines in GetTooltipLines(_rerollItemPanel.item))
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
