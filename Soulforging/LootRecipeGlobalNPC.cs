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
				ModContent.GetInstance<LootEssenceWorld>().UnlockCube(cube.item.type);
			}
			// TODO if item is soul, unlock soulforging
			return base.OnPickup(item, player);
		}
	}
}
