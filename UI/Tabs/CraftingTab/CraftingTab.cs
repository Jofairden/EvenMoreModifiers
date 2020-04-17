using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Graphics;
using Loot.Api.Mechanism;
using Loot.Api.Strategy;
using Loot.Ext;
using Loot.RollingStrategies;
using Loot.Sounds;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Common.Controls.Panel;
using Loot.UI.Tabs.Cubing;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI.Tabs.CraftingTab
{
	// Because fuck generics
	interface ICraftingTab
	{
		bool AcceptsItem(Item item);
		void GiveBackSlottedItem();
		void OverrideSlottedItem(Item newItem);
	}

	/**
	 * Defines a generic crafting tab
	 * Used with cube crafting and essence crafting
	 */
	internal abstract class CraftingTab<T> : GuiTab, ICraftingTab where T : ModItem
	{
		public override int GetPageHeight() => 100;

		internal abstract CraftingComponentButton GetComponentButton();
		protected abstract ModifierContextMethod CraftMethod { get; }

		protected virtual Dictionary<string, object> CustomData => new Dictionary<string, object>
		{
			{
				"Source", "Crafting"
			}
		};

		public virtual bool AcceptsItem(Item item)
		{
			return ItemButton?.CanTakeItem(item) ?? false;
		}

		internal CraftingComponentButton ComponentButton;
		internal GuiCraftItemButton ItemButton;
		internal RollingStrategyProperties RollingStrategyProperties;
		internal CraftingComponentSelector<T> ComponentSelector;

		protected GuiImageButton _craftButton;
		protected GuiTextPanel[] _modifierPanels;

		public override void OnInitialize()
		{
			base.OnInitialize();

			RollingStrategyProperties = new RollingStrategyProperties();

			ComponentButton = GetComponentButton();
			ComponentButton.OnItemChange += item =>
			{
				ComponentSelector.CalculateAvailableComponents(ItemButton.Item);
				if (!ItemButton.Item.IsAir && !IsStrategyAllowed())
				{
					ComponentButton.Item.TurnToAir();
				}
			};
			TabFrame.Append(ComponentButton);

			ItemButton = new GuiCraftItemButton(
				GuiButton.ButtonType.Parchment,
				hintTexture: ModContent.GetTexture("Terraria/Item_24"),
				hintText: "Place an item here"
			);
			ItemButton.CanTakeItemAction += IsStrategyAllowed;
			ItemButton.PreOnClickAction += () =>
			{
				if (!ItemButton.Item.IsAir)
				{
					LootModItem.GetInfo(ItemButton.Item).SlottedInUI = false;
				}
			};
			ItemButton.OnItemChange += item =>
			{
				ComponentSelector.CalculateAvailableComponents(ItemButton.Item);
				if (!ComponentSelector.IsAvailable(ComponentButton.Item.type))
				{
					ComponentButton.Item.TurnToAir();
				}

				if (!item.IsAir)
				{
					LootModItem.GetInfo(item).SlottedInUI = true;
				}
				UpdateModifiersInGui();
			};
			TabFrame.Append(ItemButton.Below(ComponentButton));

			_modifierPanels = new GuiTextPanel[4];
			for (int i = 0; i < 4; i++)
			{
				_modifierPanels[i] = new GuiTextPanel();
				var panel = _modifierPanels[i].RightOf(ComponentButton);
				if (i > 0)
				{
					panel.Below(_modifierPanels[i - 1]);
				}

				TabFrame.Append(panel);
			}

			Texture2D craftTexture = ModContent.GetTexture("Terraria/UI/Craft_Toggle_3");
			_craftButton = new GuiImageButton(GuiButton.ButtonType.StoneOuterBevel, craftTexture);
			_craftButton.OnClick += (evt, element) =>
			{
				if (!ComponentSelector.IsAvailable(ComponentButton.Item.type))
				{
					ComponentButton.Item.TurnToAir();
				}

				HandleCraftButtonClick(evt, element);
			};

			TabFrame.Append(_craftButton.Below(ItemButton));

			ComponentSelector = new CraftingComponentSelector<T>();
			ComponentSelector.OnComponentClick += type =>
			{
				ComponentSelector.CalculateAvailableComponents();
				if (!ComponentSelector.IsAvailable(type))
				{
					ComponentButton.Item.TurnToAir();
					return false;
				}

				ComponentButton.ChangeItem(type);
				return true;
			};
			Height.Set(Height.Pixels + ComponentSelector.Height.Pixels, 0);
			Append(ComponentSelector);
		}

		private bool CraftRequirementsMet()
		{
			bool match = !ItemButton.Item.IsAir && !ComponentButton.Item.IsAir
												&& ItemButton.CanTakeItem(ItemButton.Item)
												&& ComponentButton.CanTakeItem(ComponentButton.Item);
			if (!match) return false;

			bool hasItem = Main.LocalPlayer.inventory.Any(x => x.type == ComponentButton.Item.type)
						   || Main.mouseItem?.type == ComponentButton?.Item?.type;

			if (!hasItem) return false;
			return IsStrategyAllowed();
		}

		// TODO make this better
		private bool IsStrategyAllowed(Item overrideItem = null)
		{
			RollingStrategyProperties = new RollingStrategyProperties();
			var ctx = GetContext(overrideItem ?? ItemButton.Item);
			var strategy = ctx.Strategy;
			var preRolledLines = strategy.PreRoll(ModifierPoolMechanism.GetPool(ctx), ctx, RollingStrategyProperties);
			return preRolledLines.All(x => x.CanRoll(ctx));
		}

		public override void OnShow()
		{
			ComponentSelector?.CalculateAvailableComponents(ItemButton.Item);
			UpdateModifiersInGui();
		}

		private ModifierContext GetContext(Item item)
		{
			return new ModifierContext
			{
				Method = CraftMethod,
				Item = item,
				Player = Main.LocalPlayer,
				CustomData = this.CustomData,
				Strategy = ComponentButton.GetRollingStrategy(ItemButton.Item, RollingStrategyProperties)
			};
		}

		private void HandleCraftButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (!CraftRequirementsMet())
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				return;
			}

			RollingStrategyProperties = new RollingStrategyProperties();
			ComponentButton.RecalculateStack();

			if (ComponentButton.Item.stack <= 0)
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				ComponentButton.Item.TurnToAir();
				return;
			}

			var item = GetClonedItem(ItemButton.Item);
			var info = LootModItem.GetInfo(item);
			var strategy = ComponentButton.GetRollingStrategy(item, RollingStrategyProperties);

			// Sealed and not allowed to roll
			if (info.SealedModifiers && !(strategy is SealingRollingStrategy))
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				return;
			}

			// Roll new lines
			var context = GetContext(item);
			var rolled = strategy._Roll(ModifierPoolMechanism.GetPool(context), context, RollingStrategyProperties);
			LootModItem.GetInfo(item).Modifiers = NullModifierPool.INSTANCE; // unload previous pool
			if (rolled.Any())
			{
				item.UpdateModifiers(rolled);
			}
			strategy.PlaySoundEffect(item);
			ItemButton.Item = item;

			ConsumeComponents();
			UpdateModifiersInGui();
		}

		private Item GetClonedItem(Item toClone)
		{
			var clone = new Item();
			clone.netDefaults(toClone.type);
			// Clone to preserve modded data
			clone = clone.CloneWithModdedDataFrom(ItemButton.Item);
			// Restore prefix
			if (ItemButton.Item.prefix > 0)
			{
				clone.Prefix(toClone.prefix);
			}

			clone.stack = toClone.stack;
			return clone;
		}

		public void OverrideSlottedItem(Item newItem)
		{
			ItemButton.ChangeItem(0, newItem.Clone());
			LootModItem.GetInfo(ItemButton.Item).SlottedInUI = true;
			UpdateModifiersInGui();
			Main.PlaySound(SoundID.Grab);
		}

		private void UpdateModifiersInGui()
		{
			foreach (var panel in _modifierPanels)
			{
				panel.ResetText();
			}

			if (ItemButton.Item.IsAir || _modifierPanels == null)
			{
				return;
			}

			int i = 0;

			IEnumerable<ModifierTooltipLine[]> GetTooltipLines(Item item)
				=> LootModItem.GetActivePool(item)
					.Select(x => x.GetTooltip().Build().ToArray())
					.Take(4)
					.Where(x => x != null);

			foreach (var lines in GetTooltipLines(ItemButton.Item))
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

		private void ConsumeComponents()
		{
			bool checkMouse = true;
			// Remove stack from player's inventory
			// .Take 58 because 59th slot is MouseItem for some reason.
			foreach (Item item in Main.LocalPlayer.inventory.Take(58))
			{
				if (item.type == ComponentButton.Item.type)
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

			ComponentButton.RecalculateStack();
			ComponentSelector.CalculateAvailableComponents(ItemButton.Item);

			// No slotted cubes available
			if (ComponentButton.Item.stack <= 0)
			{
				ComponentButton.Item.TurnToAir();
			}
		}

		internal override void ToggleUI(bool visible)
		{
			base.ToggleUI(visible);
			if (!visible)
			{
				GiveBackSlottedItem();
			}
		}

		public void GiveBackSlottedItem()
		{
			if (!ItemButton?.Item?.IsAir ?? false)
			{
				// Runs only in SP or client, so this is safe
				ItemButton.Item.noGrabDelay = 0;
				Main.LocalPlayer.GetItem(Main.myPlayer, GetClonedItem(ItemButton.Item));
				ItemButton.Item.TurnToAir();
			}
		}
	}
}
