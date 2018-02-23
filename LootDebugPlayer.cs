using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	//class LootDebugGItem : GlobalItem
	//{
	//	public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	//	{
	//		EMMItem eMMItem = item.GetGlobalItem<EMMItem>();
	//		if (eMMItem.Modifier != null)
	//		{

	//		}
	//	}

	//}

	// debug player
	class LootDebugPlayer : ModPlayer
	{
		public bool debugEffect;

		public override void ResetEffects()
		{
			debugEffect = false;
		}

		public override void UpdateDead()
		{
			debugEffect = false;
		}

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			if (debugEffect)
			{
				player.meleeDamage += 0.5f;
				player.inferno = true;
				player.statDefense = 9999;
			}
		}
	}
}
