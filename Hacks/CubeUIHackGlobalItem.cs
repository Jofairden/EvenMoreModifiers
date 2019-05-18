namespace Loot.Hacks
{
	// TODO
	//public class CubeUIHackGlobalItem : GlobalItem
	//{
	//	// Needed to enable right click for possible items
	//	public override bool CanRightClick(Item item)
	//	{
	//		if (!RightClickFunctionalityRequirements(item))
	//		{
	//			return false;
	//		}

	//		var ui = Loot.Instance.CubeInterface.CurrentState as CubeUI;
	//		if (ui == null)
	//		{
	//			return false;
	//		}

	//		return ui.Visible && ui.IsItemValidForUISlot(item);
	//	}

	//	private bool RightClickFunctionalityRequirements(Item item)
	//	{
	//		if (ModSupport.GetSupport<WingSlotSupporter>().IsInvalid && item.wingSlot > 0)
	//		{
	//			return false;
	//		}

	//		return !PlayerInput.WritingText
	//			   && Main.hasFocus
	//			   && Main.keyState.IsKeyDown(Keys.LeftControl)
	//			   && Loot.Instance.CubeInterface.CurrentState != null;
	//	}

	//	// Auto slot item in UI if possible
	//	public override void RightClick(Item item, Player player)
	//	{
	//		// We need to rerun checks here because our forced right click above
	//		// becomes meaningless to items that can already be right clicked
	//		// meaning that items such as goodie bags will end up in this hook
	//		// regardless of our forced right click functionality in CanRightClick
	//		// by which we need to assume any possible item can be passed into this hook
	//		var ui = Loot.Instance.CubeInterface.CurrentState as CubeUI;
	//		if (ui == null)
	//		{
	//			return;
	//		}

	//		if (RightClickFunctionalityRequirements(item) && !(item.modItem is MagicalCube) && ui.Visible && ui.IsItemValidForUISlot(item))
	//		{
	//			if (ui is CubeRerollUI)
	//			{
	//				RerollUITakeItem(ui as CubeRerollUI, item);
	//			}
	//			else if (ui is CubeSealUI)
	//			{
	//				SealUITakeItem(ui as CubeSealUI, item);
	//			}

	//			// else // item has innate right click or mod allows it, do nothing
	//		}
	//	}

	//	private void RerollUITakeItem(CubeRerollUI ui, Item item)
	//	{
	//		SwapItemSlot(item, ui, ui._rerollItemPanel, ui.UpdateModifierLines);
	//	}

	//	private void SealUITakeItem(CubeSealUI ui, Item item)
	//	{
	//		SwapItemSlot(item, ui, ui._itemPanel);
	//	}

	//	// Swap item from given slot in UI
	//	private void SwapItemSlot(Item item, VisibilityUI ui, UIInteractableItemPanel itemPanel, Action extraAction = null)
	//	{
	//		if (itemPanel != null && itemPanel.CanTakeItem(item))
	//		{
	//			if (!itemPanel.item.IsAir)
	//			{
	//				EMMItem.GetItemInfo(itemPanel.item).SlottedInCubeUI = false;
	//				Main.LocalPlayer.QuickSpawnClonedItem(itemPanel.item, itemPanel.item.stack);
	//			}

	//			Main.PlaySound(SoundID.Grab);
	//			itemPanel.item = item.Clone();
	//			EMMItem.GetItemInfo(itemPanel.item).SlottedInCubeUI = true;

	//			extraAction?.Invoke();
	//		}
	//		else
	//		{
	//			item.stack++;
	//		}
	//	}

	//	// Give notice how to slot
	//	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	//	{
	//		var ui = Loot.Instance.CubeInterface;

	//		if (!(ui?.CurrentState is CubeUI cubeUI))
	//		{
	//			return;
	//		}

	//		if ((ModSupport.GetSupport<WingSlotSupporter>().IsInvalid && item.wingSlot > 0) // block wings if low version if wingslot
	//			|| !cubeUI.IsItemValidForUISlot(item)
	//			|| cubeUI.IsSlottedItemInCubeUI())
	//		{
	//			return;
	//		}

	//		var i = tooltips.FindIndex(x => x.mod.Equals("Terraria") && x.Name.Equals("ItemName"));
	//		if (i != -1)
	//		{
	//			tooltips[i].text += " (control right click to slot into UI)";
	//		}
	//	}
	//}
}
