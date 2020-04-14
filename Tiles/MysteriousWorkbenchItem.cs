using Terraria;
using Terraria.ModLoader;

namespace Loot.Tiles
{
	internal sealed class MysteriousWorkbenchItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			DisplayName.SetDefault("MysteriousWorkbenchItem");
		}

		public override void SetDefaults()
		{
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.maxStack = 99;
			item.consumable = true;
			item.width = 40;
			item.height = 30;
			item.value = Item.sellPrice(0, 0, 0, 0);
			item.createTile = ModContent.TileType<MysteriousWorkbench>();
		}
	}
}
