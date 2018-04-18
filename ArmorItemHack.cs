using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private bool IsArmor(Item item) => (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1) && !item.vanity;
		public override bool CanEquipAccessory(Item item, Player player, int slot) => !item.IsAir && !IsArmor(item);

		public override void UpdateInventory(Item item, Player player)
		{
			if (!item.IsAir && IsArmor(item))
			{
				item.accessory = true;
			}
		}

		public override void PostReforge(Item item)
		{
			if (IsArmor(item))
			{
				Item iRef = new Item();
				iRef.netDefaults(item.netID);
				item = iRef.CloneWithModdedDataFrom(item);
			}
		}


	}
}
