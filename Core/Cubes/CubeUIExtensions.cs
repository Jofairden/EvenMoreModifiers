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
			CubeUI.Visible
			&& !PlayerInput.WritingText
			&& Main.hasFocus
			&& Main.keyState.IsKeyDown(Keys.LeftControl)
			&& Loot.Instance.cubeUI._rerollItemPanel.CanTakeItem(item);

		public override void RightClick(Item item, Player player)
		{
			if (CubeUI.Visible && item.IsModifierRollableItem())
			{
				var ui = Loot.Instance.cubeUI;

				// If there is an item slotted
				if (ui._rerollItemPanel != null
				    && !ui._rerollItemPanel.item.IsAir)
				{
					Main.LocalPlayer.QuickSpawnClonedItem(ui._rerollItemPanel.item, ui._rerollItemPanel.item.stack);
				}

				if (ui._rerollItemPanel != null)
				{
					Main.PlaySound(SoundID.Grab);
					ui._rerollItemPanel.item = item.Clone();
					ui.UpdateModifierLines();
				}

//			item.TurnToAir();
			}
		}
	}
}