using System;
using System.Collections.Generic;
using Loot.Api.Cubes;
using Loot.UI;
using Loot.UI.Tabs.Cubing;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Loot.ModSupport
{
	internal class WingSlotSupport : ModSupport
	{
		public override string ModName => "WingSlot";

		public bool IsInvalid;

		// "Adds" this check for the item to be equipped into the wing slot
		// If it returns true, it stops the item being equipped
		private static bool WingSlotHandler()
			=> Loot.Instance.GuiInterface?.CurrentState is GuiTabWindow tabUi
			   && tabUi.Visible;

		public override bool CheckValidity(Mod mod)
		{
			IsInvalid = mod.Version < new Version(1, 6, 1);
			return !IsInvalid;
		}

		public override void AddClientSupport(Mod mod)
		{
			mod.Call("add", (Func<bool>)WingSlotHandler);
		}

		private static bool RightClickFunctionalityRequirements(Item item)
		{
			if (ModSupportTunneler.GetSupport<WingSlotSupport>().IsInvalid && item.wingSlot > 0)
				return false;

			return !PlayerInput.WritingText
				   && Main.hasFocus
				   && Main.keyState.IsKeyDown(Keys.LeftControl)
				   && Loot.Instance.GuiInterface.CurrentState != null;
		}

		private static void SwapItems(GuiCubingTab cubingTab, Item item)
		{
			var itemPanel = cubingTab._itemButton;
			if (!itemPanel.Item.IsAir)
			{
				LootModItem.GetInfo(itemPanel.Item).SlottedInCubeUI = false;
				Main.LocalPlayer.QuickSpawnClonedItem(itemPanel.Item, itemPanel.Item.stack);
			}

			Main.PlaySound(SoundID.Grab);
			itemPanel.Item = item.Clone();
			LootModItem.GetInfo(itemPanel.Item).SlottedInCubeUI = true;
		}

		public class WingSlotSupportGlobalItem : GlobalItem
		{
			// Needed to enable right click for possible items
			public override bool CanRightClick(Item item)
			{
				if (!RightClickFunctionalityRequirements(item))
					return false;

				if (!(Loot.Instance.GuiInterface.CurrentState is GuiTabWindow ui)
					|| !(ui.GetCurrentTab() is GuiCubingTab cubingTab))
					return false;

				return ui.Visible && cubingTab.AcceptsItem(item);
			}

			// Auto slot item in UI if possible
			public override void RightClick(Item item, Player player)
			{
				// We need to rerun checks here because our forced right click above
				// becomes meaningless to items that can already be right clicked
				// meaning that items such as goodie bags will end up in this hook
				// regardless of our forced right click functionality in CanRightClick
				// by which we need to assume any possible item can be passed into this hook
				if (!(Loot.Instance.GuiInterface.CurrentState is GuiTabWindow ui)
					|| !(ui.GetCurrentTab() is GuiCubingTab cubingTab))
					return;

				if (RightClickFunctionalityRequirements(item) && !(item.modItem is MagicalCube) && ui.Visible && cubingTab.AcceptsItem(item))
				{
					SwapItems(cubingTab, item);
					// else // item has innate right click or mod allows it, do nothing
				}
			}

			// Give notice how to slot
			public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
			{
				if (!(Loot.Instance.GuiInterface.CurrentState is GuiTabWindow ui)
				    || !(ui.GetCurrentTab() is GuiCubingTab cubingTab))
					return;

				if ((ModSupportTunneler.GetSupport<WingSlotSupport>().IsInvalid && item.wingSlot > 0) // block wings if low version if wingslot
				    || !cubingTab.AcceptsItem(item)
				    || LootModItem.GetInfo(item).SlottedInCubeUI)
					return;

				var i = tooltips.FindIndex(x => x.mod.Equals("Terraria") && x.Name.Equals("ItemName"));
				if (i != -1) tooltips[i].text += " (control right click to slot into UI)";
			}
		}
	}
}
