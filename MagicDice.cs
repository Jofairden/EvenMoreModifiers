using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// This item is used to (re)roll stats on items
	/// </summary>
	class MagicDice : ModItem
	{
		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.value = Item.sellPrice(gold: 1);
			item.rare = 3;
		}

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("This magic dice can enchant a piece of equipment... try your luck");
		}

		// @todo: open slot ui
	}
}
