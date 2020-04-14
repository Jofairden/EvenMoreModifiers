using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Loot.Cubes
{
	// This is a temporary file for cube testing

	public class CubeShop : GlobalNPC
	{
		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Wizard || type == NPCID.TravellingMerchant)
			{
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PoorCube>());
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CubeOfSealing>());
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BlackCube>());
			}
		}
	}
}
