using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Hacks
{
	/// <summary>
	/// Can detect if an item was activated via cheats
	/// </summary>
	public sealed class CheatedItemHackGlobalItem : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		/// <summary>
		/// Keeps track of if the item was activated (by another mod)
		/// In this case activated means giving its regular bonuses
		/// Example: anti social in vanity slots
		/// </summary>
		public bool IsCheated { get; internal set; }

		public bool ShouldBeIgnored(Item item, Player player)
			=> !IsCheated && IsInVanitySot(item, player)
			   || IsCheated && !IsInVanitySot(item, player) && !IsInInventory(item, player);

		public bool IsInInventory(Item item, Player player)
			=> player.inventory.Any(x => x.IsTheSameAs(item));

		/// <summary>
		/// Returns if the item is in a player's vanity slot
		/// </summary>
		public bool IsInVanitySot(Item item, Player player)
			=> player.armor.Skip(13).Any(x => x.IsTheSameAs(item));

		private bool IsNotEquippedAtAll(Item item, Player player)
			=> !player.armor.Any(x => x.IsTheSameAs(item));

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			if (IsInVanitySot(item, player) || IsNotEquippedAtAll(item, player))
				IsCheated = true;
			else
				IsCheated = false;
		}

		public override void UpdateEquip(Item item, Player player)
		{
			if (IsInVanitySot(item, player) || IsNotEquippedAtAll(item, player))
				IsCheated = true;
			else
				IsCheated = false;
		}

		public override void UpdateInventory(Item item, Player player)
		{
			IsCheated = false;
		}

		public override void PostUpdate(Item item)
		{
			IsCheated = false;
		}

		public static CheatedItemHackGlobalItem GetInfo(Item item)
			=> item.GetGlobalItem<CheatedItemHackGlobalItem>();
	}
}
