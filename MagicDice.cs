using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// This item is used to (re)roll stats on items
	/// </summary>
	public class MagicDice : ModItem
	{
		public override bool Autoload(ref string name)
		{
			return false;
		}

		public override void SetDefaults()
		{
			//item.maxStack = 1;
			item.maxStack = 999;
			item.consumable = true;
			item.value = Item.sellPrice(gold: 1);
			item.rare = 3;
			//EMMItem.GetItemInfo(item).CustomReforgeMode = CustomReforgeMode.ForceWeapon | CustomReforgeMode.Custom;
		}

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("This magic dice can enchant a piece of equipment... try your luck");
		}

		//public void EMMCustomReforge()
		//{
		//	item.rare = 10;
		//	item.damage *= 2;
		//}

		// @todo: open slot ui
	}
}
