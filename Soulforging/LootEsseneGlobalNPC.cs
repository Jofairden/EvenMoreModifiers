using Terraria;
using Terraria.ModLoader;

namespace Loot.Soulforging
{
	internal class LootEsseneGlobalNPC : GlobalNPC
	{
		public override void NPCLoot(NPC npc)
		{
			var player = Main.LocalPlayer.GetModPlayer<LootEssencePlayer>();
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
