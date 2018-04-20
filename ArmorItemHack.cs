using Loot.Modifiers;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// The following code is a giant hack and should not be attempted
	/// A new hook is TML is awaited, to allow for such behaviour without hacks
	/// </summary>
	class ArmorItemHack : GlobalItem
	{
		public override bool CanEquipAccessory(Item item, Player player, int slot) => !item.IsAir && !ArmorModifier.IsArmor(item);

		public override void UpdateInventory(Item item, Player player)
		{
			if (!item.IsAir && ArmorModifier.IsArmor(item))
			{
				item.accessory = true;
			}
		}

		public override void PostReforge(Item item)
		{
			if (ArmorModifier.IsArmor(item))
			{
				Item iRef = new Item();
				iRef.netDefaults(item.netID);
				item = iRef.CloneWithModdedDataFrom(item);
			}
		}
	}
}
