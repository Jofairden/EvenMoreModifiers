using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public class CubeShop : GlobalNPC
	{
		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Wizard)
			{
				shop.item[nextSlot++].SetDefaults(Loot.Instance.ItemType<BasicCube>());
				shop.item[nextSlot++].SetDefaults(Loot.Instance.ItemType<CubeOfSealing>());
			}
		}
	}
}