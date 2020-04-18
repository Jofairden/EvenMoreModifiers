using Terraria;

namespace Loot.UI.Tabs.CraftingTab
{
	class CraftingComponentLink
	{
		public enum ComponentSource
		{
			Soulforge,
			Inventory,
			Dynamic
		}

		public Item Component;
		public ComponentSource Source;

		public CraftingComponentLink(Item item, ComponentSource source)
		{
			Component = item;
			Source = source;
		}

		public bool EnoughAvailable(int amount) => Component.stack >= amount;

	}
}
