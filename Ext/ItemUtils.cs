using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Ext
{
	public static class ItemUtils
	{
		public static bool IsModifierRollableItem(this Item item) => item.maxStack == 1 && item.IsWeapon() || item.IsAccessory() || item.IsArmor();
		public static bool IsWeapon(this Item item) => item.damage > 0;
		public static bool IsAccessory(this Item item) => item.accessory && !item.vanity;
		public static bool IsArmor(this Item item) => (item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0) && !item.vanity;

		public static IEnumerable<T> GetDistinctModItems<T>(this Item[] inventory) where T : ModItem
			=> inventory
				.Where(x => !x.IsAir
				            && x.modItem is T)
				.GroupBy(x => x.type)
				.Select(g => (T) g.First().modItem);

		public static int CountModItemStack<T>(this Item[] inventory, bool includeMouseItem = false) where T : ModItem
			=> CountItemStack(inventory, includeMouseItem, item => item.modItem is T);

		public static int CountItemStack(this Item[] inventory, int type, bool includeMouseItem = false)
			=> CountItemStack(inventory, includeMouseItem, item => item.type == type);

		public static int CountItemStack(this Item[] inventory, bool includeMouseItem, Func<Item, bool> predicate)
		{
			int size = includeMouseItem ? 59 : 58;
			return Main.LocalPlayer.inventory.Take(size)
				.Where(predicate)
				.Select(x => x.stack)
				.Sum();
		}
	}
}
