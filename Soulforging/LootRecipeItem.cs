using Loot.Api.Cubes;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Soulforging
{
	internal class LootRecipeItem : GlobalItem
	{
		public override void UpdateInventory(Item item, Player player)
		{
			if (item.modItem is MagicalCube cube)
			{
				ModContent.GetInstance<LootEssenceWorld>().UnlockCube(cube.item.type);
			}
			// TODO if item is soul, unlock soulforging
		}
	}
}
