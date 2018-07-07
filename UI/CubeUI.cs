using Terraria;
using Terraria.UI;

namespace Loot.UI
{
	public abstract class CubeUI : DraggableUIState
	{
		public abstract bool IsItemValidForUISlot(Item item);
		public abstract bool IsSlottedItemInCubeUI();
		public abstract Item SlottedItem { get; }
	}
}