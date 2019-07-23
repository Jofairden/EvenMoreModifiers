using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Hacks
{
	// This is here so old (alpha) classes convert to the new classes
	internal sealed class EMMItem : GlobalItem
	{
		public sealed override bool NeedsSaving(Item item) => false;

		public sealed override void Load(Item item, TagCompound tag)
		{
			LootModItem.GetInfo(item).Load(item, tag);
		}

		public sealed override TagCompound Save(Item item)
		{
			return null;
		}
	}

	internal sealed class EMMWorld : ModWorld
	{
		public sealed override TagCompound Save()
		{
			return null;
		}

		public sealed override void Load(TagCompound tag)
		{
			mod.GetModWorld<LootModWorld>().Load(tag);
		}
	}
}
