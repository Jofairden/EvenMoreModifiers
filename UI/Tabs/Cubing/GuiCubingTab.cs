using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Loot.Cubes;
using Loot.Sounds;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Common.Controls.Panel;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI.Tabs.Cubing
{
	/// <summary>
	/// The tab allows using a magical cube on the slotted item to modify its potential
	/// </summary>
	internal class GuiCubingTab : GuiTab
	{
		public override string Header => "Cubing";
		public override int GetPageHeight()
		{
			return 100;
		}

		internal GuiCubeButton _cubeButton;
		internal GuiCubeItemButton _itemButton;
		internal RollingStrategyProperties RollingStrategyProperties;
		internal GuiCubeSelector _guiCubeSelector;

		private GuiImageButton _rerollButton;
		private GuiTextPanel[] _modifierPanels;

		public override void OnInitialize()
		{
			base.OnInitialize();

			RollingStrategyProperties = new RollingStrategyProperties();

			_cubeButton = new GuiCubeButton(
				GuiButton.ButtonType.StoneOuterBevel,
				hintTexture: Loot.Instance.GetTexture("Api/Cubes/MagicalCube"),
				hintText: "Place a cube here"
			);
			TabFrame.Append(_cubeButton);

			_itemButton = new GuiCubeItemButton(
				GuiButton.ButtonType.Parchment,
				hintTexture: ModContent.GetTexture("Terraria/Item_24"),
				hintText: "Place an item to reroll here"
			);
			_itemButton.OnClick += (evt, element) => { UpdateModifiersInGui(); };
			TabFrame.Append(_itemButton.Below(_cubeButton));

			_modifierPanels = new GuiTextPanel[4];
			for (int i = 0; i < 4; i++)
			{
				_modifierPanels[i] = new GuiTextPanel();
				var panel = _modifierPanels[i].RightOf(_cubeButton);
				if (i > 0)
				{
					panel.Below(_modifierPanels[i - 1]);
				}
				TabFrame.Append(panel);
			}

			Texture2D rerollTexture = ModContent.GetTexture("Terraria/UI/Craft_Toggle_3");
			_rerollButton = new GuiImageButton(GuiButton.ButtonType.StoneOuterBevel, rerollTexture);
			_rerollButton.OnClick += HandleRerollButtonClick;

			TabFrame.Append(_rerollButton.Below(_itemButton));

			_guiCubeSelector = new GuiCubeSelector
			{
				HAlign = 0.5f,
				VAlign = 1f
			};
			Height.Set(Height.Pixels + _guiCubeSelector.Height.Pixels, 0);
			Append(_guiCubeSelector);
		}

		public bool AcceptsCube(Item item)
			=> _cubeButton.CanTakeItem(item);

		public bool AcceptsItem(Item item)
			=> _itemButton.CanTakeItem(item);

		private bool MatchesRerollRequirements()
		{
			bool match = !_itemButton.Item.IsAir && !_cubeButton.Item.IsAir
						 && _itemButton.CanTakeItem(_itemButton.Item)
						 && _cubeButton.CanTakeItem(_cubeButton.Item);
			//&& !EMMItem.GetItemInfo(_itemButton.Item).SealedModifiers // omitted for now

			bool hasItem = Main.LocalPlayer.inventory.Any(x => x.type == _cubeButton.Item.type)
						   || Main.mouseItem?.type == _cubeButton?.Item?.type;

			return match && hasItem;
		}

		private void HandleRerollButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (MatchesRerollRequirements())
			{
				RollingStrategyProperties = new RollingStrategyProperties();
				_cubeButton.RecalculateStack();

				if (_cubeButton.Item.stack <= 0)
				{
					SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
					_cubeButton.Item.TurnToAir();
					return;
				}

				var info = LootModItem.GetInfo(_itemButton.Item);
				if (_cubeButton.Item.modItem is CubeOfSealing)
				{
					info.SealedModifiers = !info.SealedModifiers;
					SoundHelper.PlayCustomSound(info.SealedModifiers ? SoundHelper.SoundType.GainSeal : SoundHelper.SoundType.LoseSeal);
					ConsumeCubes();
				}
				else if (info.SealedModifiers)
				{
					SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				}
				else
				{
					// Refresh item
					Item newItem = GetClonedItem(_itemButton.Item);
					LootModItem.GetInfo(newItem).ModifierPool = null; // unload previous pool

					// reroll pool
					RerollModifiers(newItem);
					UpdateModifiersInGui();
					ConsumeCubes();
					Main.PlaySound(SoundID.Item37, -1, -1);
				}
			}
			else
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
			}
		}

		private Item GetClonedItem(Item toClone)
		{
			var clone = new Item();
			clone.netDefaults(toClone.type);
			// Clone to preserve modded data
			clone = clone.CloneWithModdedDataFrom(_itemButton.Item);
			// Restore prefix
			if (_itemButton.Item.prefix > 0)
			{
				clone.Prefix(toClone.prefix);
			}
			clone.stack = toClone.stack;
			return clone;
		}

		private void RerollModifiers(Item newItem)
		{
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnCubeReroll,
				Item = newItem,
				Player = Main.LocalPlayer,
				CustomData = new Dictionary<string, object>
				{
					{"Source", "CubeRerollUI"}
				},
				Strategy = _cubeButton.GetRollingStrategy(_itemButton.Item, RollingStrategyProperties)
			};

			var pool = LootModItem.GetInfo(newItem).RollNewPool(ctx, RollingStrategyProperties);
			pool?.ApplyModifiers(newItem);
			_itemButton.Item = newItem.Clone();
		}

		private void UpdateModifiersInGui()
		{
			foreach (var panel in _modifierPanels)
			{
				panel.ResetText();
			}

			if (_itemButton.Item.IsAir
				|| _modifierPanels == null)
			{
				return;
			}

			int i = 0;

			IEnumerable<ModifierTooltipLine[]> GetTooltipLines(Item item)
				=> LootModItem.GetActivePool(item)
					.Select(x => x.GetTooltip().Build().ToArray())
					.Take(4)
					.Where(x => x != null);

			foreach (var lines in GetTooltipLines(_itemButton.Item))
			{
				string line = lines.Aggregate("", (current, tooltipLine) => current + $"{tooltipLine.Text} ");
				line = line.TrimEnd();
				var measure = Main.fontMouseText.MeasureString(line);
				if (measure.X >= _modifierPanels[i].Width.Pixels + SPACING * 4)
				{
					_modifierPanels[i].SetHoverText(line);
					line = Main.fontMouseText.CreateWrappedText(line, _modifierPanels[i].Width.Pixels)
						.Split('\n')[0] + "...";
				}
				else
				{
					_modifierPanels[i].SetHoverText(null);
				}
				_modifierPanels[i].UpdateText(line);
				i++;
			}

			Recalculate();
		}

		private void ConsumeCubes()
		{
			bool checkMouse = true;
			// Remove stack from player's inventory
			// .Take 58 because 59th slot is MouseItem for some reason.
			foreach (Item item in Main.LocalPlayer.inventory.Take(58))
			{
				if (item.type == _cubeButton.Item.type)
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

			_cubeButton.RecalculateStack();
			_guiCubeSelector.DetermineAvailableCubes();

			// No slotted cubes available
			if (_cubeButton.Item.stack <= 0)
			{
				_cubeButton.Item.TurnToAir();
			}
		}

		internal override void ToggleUI(bool visible)
		{
			base.ToggleUI(visible);
			if (visible)
			{
				UpdateModifiersInGui();
				_guiCubeSelector.DetermineAvailableCubes();
			}
			else
			{
				GiveBackSlottedItem();
			}
		}

		public void GiveBackSlottedItem()
		{
			if (!_itemButton.Item?.IsAir ?? false)
			{
				// Runs only in SP or client, so this is safe
				_itemButton.Item.noGrabDelay = 0;
				Main.LocalPlayer.GetItem(Main.myPlayer, GetClonedItem(_itemButton.Item));
				_itemButton.Item.TurnToAir();
			}
		}
	}
}
