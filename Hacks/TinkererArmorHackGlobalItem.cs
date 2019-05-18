using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Loot.Hacks
{
	/// <summary>
	/// This class forces armor equips to be able to be reforged as accessories
	/// The actual activation to allow them to be slotted is in <see cref="TinkererArmorHackModPlayer"/>
	/// </summary>
	public sealed class TinkererArmorHackGlobalItem : GlobalItem
	{
		// Forces an accessory tinker
		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			var info = LootModItem.GetInfo(item);
			if (info.JustTinkerModified)
			{
				item.accessory = true;
			}

			return -1;
		}

		// Reset accessory state after reforge
		public override void PostReforge(Item item)
		{
			var info = LootModItem.GetInfo(item);
			if (info.JustTinkerModified)
			{
				item.accessory = false;
			}
		}
	}
}
