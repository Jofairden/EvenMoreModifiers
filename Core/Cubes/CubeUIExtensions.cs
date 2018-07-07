using System;
using System.Collections.Generic;
using Loot.UI;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public class CubeUIExtensions : GlobalItem
	{
		public override bool CanRightClick(Item item)
		{
			if (PlayerInput.WritingText
			    || !Main.hasFocus
			    || !Main.keyState.IsKeyDown(Keys.LeftControl)
			    || Loot.Instance.CubeInterface.CurrentState == null) 
				return false;

			var ui = Loot.Instance.CubeInterface.CurrentState as CubeUI;
			if (ui == null) return false;

			return ui.Visible && ui.IsItemValidForUISlot(item);
		}

		// Auto slot item in UI if possible
		public override void RightClick(Item item, Player player)
		{
			if (!(item.modItem is MagicalCube))
			{
				var ui = Loot.Instance.CubeInterface;

				// ReSharper disable once ConvertIfStatementToSwitchStatement
				// ^ needs C#7
				if (ui.CurrentState is CubeRerollUI) RerollUITakeItem(ui.CurrentState as CubeRerollUI, item);
				else if (ui.CurrentState is CubeSealUI) SealUITakeItem(ui.CurrentState as CubeSealUI, item);
				else item.stack++;
			}
		}

		private void RerollUITakeItem(CubeRerollUI ui, Item item)
		{
			SwapItemSlot(item, ui, ui._rerollItemPanel, ui.UpdateModifierLines);
		}

		private void SealUITakeItem(CubeSealUI ui, Item item)
		{
			SwapItemSlot(item, ui, ui._itemPanel);
		}

		// Swap item from given slot in UI
		private void SwapItemSlot(Item item, VisibilityUI ui, UIInteractableItemPanel itemPanel, Action extraAction = null)
		{
			if (itemPanel != null && itemPanel.CanTakeItem(item))
			{
				if (!itemPanel.item.IsAir)
				{
					EMMItem.GetItemInfo(itemPanel.item).SlottedInCubeUI = false;
					Main.LocalPlayer.QuickSpawnClonedItem(itemPanel.item, itemPanel.item.stack);
				}

				Main.PlaySound(SoundID.Grab);
				itemPanel.item = item.Clone();
				EMMItem.GetItemInfo(itemPanel.item).SlottedInCubeUI = true;

				extraAction?.Invoke();
			}
			else
			{
				item.stack++;
			}
		}

		// Give notice how to slot
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			var ui = Loot.Instance.CubeInterface;

			var cubeUI = ui?.CurrentState as CubeUI;
			if (cubeUI != null)
			{
				if (!cubeUI.IsItemValidForUISlot(item)
				    || cubeUI.IsSlottedItemInCubeUI()) 
					return;

				var i = tooltips.FindIndex(x => x.mod.Equals("Terraria") && x.Name.Equals("ItemName"));
				if (i != -1)
				{
					tooltips[i].text += " (control right click to slot into UI)";
				}
			}
		}
	}
}