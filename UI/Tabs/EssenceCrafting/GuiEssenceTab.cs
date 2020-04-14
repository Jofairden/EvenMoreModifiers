using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Mechanism;
using Loot.Api.Strategy;
using Loot.Ext;
using Loot.RollingStrategies;
using Loot.Sounds;
using Loot.UI.Common;
using Loot.UI.Common.Controls.Button;
using Loot.UI.Tabs.Cubing;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI.Tabs.EssenceCrafting
{
	internal class GuiEssenceTab : GuiTab
	{
		public override string Header => "Essencecrafting";
		public override int GetPageHeight()
		{
			return 300;
		}

		internal RollingStrategyProperties RollingStrategyProperties;
		internal GuiEssenceButton _essenceButton;
		internal GuiCubeItemButton _itemButton;
		private GuiImageButton _rerollButton;

		public override void OnInitialize()
		{
			base.OnInitialize();

			_essenceButton = new GuiEssenceButton(
				GuiButton.ButtonType.StoneOuterBevel,
				hintTexture: Assets.Textures.PlaceholderTexture,
				hintText: "Place an essence here"
			);
			TabFrame.Append(_essenceButton);

			_itemButton = new GuiCubeItemButton(
				GuiButton.ButtonType.Parchment,
				hintTexture: ModContent.GetTexture("Terraria/Item_24"),
				hintText: "Place an item to reroll here"
			);
			// _itemButton.OnClick += (evt, element) => { UpdateModifiersInGui(); };
			TabFrame.Append(_itemButton.Below(_essenceButton));

			Texture2D rerollTexture = ModContent.GetTexture("Terraria/UI/Craft_Toggle_3");
			_rerollButton = new GuiImageButton(GuiButton.ButtonType.StoneOuterBevel, rerollTexture);
			_rerollButton.OnClick += HandleRerollButtonClick;

			TabFrame.Append(_rerollButton.Below(_itemButton));
		}


		private bool MatchesRerollRequirements()
		{
			bool match = !_itemButton.Item.IsAir && !_essenceButton.Item.IsAir
												 && _itemButton.CanTakeItem(_itemButton.Item)
												 && _essenceButton.CanTakeItem(_essenceButton.Item);

			bool hasItem = Main.LocalPlayer.inventory.Any(x => x.type == _essenceButton.Item.type)
						   || Main.mouseItem?.type == _essenceButton?.Item?.type;

			return match && hasItem;
		}

		private void HandleRerollButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (!MatchesRerollRequirements())
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				return;
			}

			RollingStrategyProperties = new RollingStrategyProperties();

			if (_essenceButton.Item.stack <= 0)
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				_essenceButton.Item.TurnToAir();
				return;
			}

			var item = GetClonedItem(_itemButton.Item);
			var info = LootModItem.GetInfo(item);
			var strategy = _essenceButton.GetRollingStrategy(item, RollingStrategyProperties);

			// Sealed and not allowed to roll
			if (info.SealedModifiers && !(strategy is SealingRollingStrategy))
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				return;
			}

			// Roll new lines
			var context = new ModifierContext
			{
				Method = ModifierContextMethod.OnCubeReroll,
				Item = item,
				Player = Main.LocalPlayer,
				CustomData = new Dictionary<string, object>
				{
					{"Source", "EssenceCraftUI"}
				},
				Strategy = _essenceButton.GetRollingStrategy(_itemButton.Item, RollingStrategyProperties)
			};
			var rolled = strategy._Roll(ModifierPoolMechanism.GetPool(context), context, RollingStrategyProperties);
			LootModItem.GetInfo(item).Modifiers = NullModifierPool.INSTANCE; // unload previous pool
			if (rolled.Any())
			{
				item.UpdateModifiers(rolled);
			}
			strategy.PlaySoundEffect(item);
			_itemButton.Item = item;

			ConsumeEssences();
			if (_essenceButton.Item.stack <= 0)
			{
				_essenceButton.Item.TurnToAir();
			}
			//UpdateModifiersInGui();
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

		private void ConsumeEssences()
		{
			bool checkMouse = true;
			// Remove stack from player's inventory
			// .Take 58 because 59th slot is MouseItem for some reason.
			foreach (Item item in Main.LocalPlayer.inventory.Take(58))
			{
				if (item.type == _essenceButton.Item.type)
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
		}

	}
}
