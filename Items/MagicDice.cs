using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Loot.Items
{
    public class MagicDice : ModItem
    {
        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.value = 10000;
            item.rare = 3;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Dice");
            Tooltip.SetDefault("While holding, right click a piece of equipment in your inventory to reroll modifiers");
        }
        public override void AddRecipes()
        {

        }
    }
}