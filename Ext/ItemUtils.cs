using Terraria;

namespace Loot.Ext
{
	public static class ItemUtils
	{
		public static bool IsModifierRollableItem(this Item item) => item.maxStack == 1 && item.IsWeapon() || item.IsAccessory() || item.IsArmor();
		public static bool IsWeapon(this Item item) => item.damage > 0;
		public static bool IsAccessory(this Item item) => item.accessory && !item.vanity;
		public static bool IsArmor(this Item item) => (item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0) && !item.vanity;
	}
}
