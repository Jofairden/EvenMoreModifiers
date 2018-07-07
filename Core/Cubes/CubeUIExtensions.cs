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
		public override bool CanRightClick(Item item) =>
			!PlayerInput.WritingText
			&& Main.hasFocus
			&& Main.keyState.IsKeyDown(Keys.LeftControl)
			&& Loot.Instance.CubeInterface.CurrentState != null
			&& ((Loot.Instance.CubeInterface.CurrentState as VisibilityUI)?.Visible ?? false)
			&& Loot.Instance.CubeInterface.CurrentState is CubeUI;


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

			if (ui?.CurrentState != null)
			{
				if (ui.CurrentState is CubeSealUI || ui.CurrentState is CubeRerollUI)
				{
					if (ui.CurrentState is CubeSealUI)
					{
						var state = (CubeSealUI) ui.CurrentState;
						if (!state._itemPanel.CanTakeItem(item)
						    || !state._itemPanel.item.IsAir && EMMItem.GetItemInfo(state._itemPanel.item).SlottedInCubeUI)
							return;
					}
					else if (ui.CurrentState is CubeRerollUI)
					{
						var state = (CubeRerollUI) ui.CurrentState;
						if (!state._rerollItemPanel.CanTakeItem(item)
						    || !state._rerollItemPanel.item.IsAir && EMMItem.GetItemInfo(state._rerollItemPanel.item).SlottedInCubeUI)
							return;
					}

					var i = tooltips.FindIndex(x => x.mod.Equals("Terraria") && x.Name.Equals("ItemName"));
					if (i != -1)
					{
						tooltips[i].text += " (control right click to slot into UI)";
					}
				}
			}
		}
	}
}