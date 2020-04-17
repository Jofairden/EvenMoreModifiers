using Loot.Api.Cubes;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Soulforging
{
	internal class LootRecipeGlobalNPC : GlobalItem
	{
		public override bool OnPickup(Item item, Player player)
		{
			if (item.modItem is MagicalCube cube)
			{
				ModContent.GetInstance<LootRecipeWorld>().UnlockCube(cube.item.type);
			}
			return base.OnPickup(item, player);
		}
	}
}
