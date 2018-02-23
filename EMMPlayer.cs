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
	/// Holds player-entity data and handles it
	/// </summary>
	public class EMMPlayer : ModPlayer
	{
		public static EMMPlayer PlayerInfo(Player player) => player.GetModPlayer<EMMPlayer>();

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			foreach (var i in player.inventory.Where(x => !x.IsAir))
			{
				EMMItem.GetItemInfo(i).UpdatePlayer(this, Modifiers.ModifierItemStatus.Inventory);
			}

			foreach (var i in player.armor.Where(x => !x.IsAir))
			{
				EMMItem.GetItemInfo(i).UpdatePlayer(this, Modifiers.ModifierItemStatus.Equipped);
			}
		}
	}
}
