using Terraria;
using Terraria.ModLoader;

namespace Loot.Soulforging
{
	internal class LootEsseneGlobalNPC : GlobalNPC
	{
		public override void NPCLoot(NPC npc)
		{
			var player = Main.LocalPlayer.GetModPlayer<LootEssencePlayer>();
			if (!ModContent.GetInstance<LootEssenceWorld>().SoulforgingUnlocked) return;

			// TODO did this player kill the npc though? is the player close enough?
			player.KillCount++;

			if (npc.boss)
			{
				player.GainEssence(10);
			}
			
			if (player.KillCount >= 9)
			{
				player.KillCount = 0;
				player.GainEssence(1);
			}
		}
	}
}
