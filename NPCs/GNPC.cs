using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Loot.NPCs
{
	public class GNPC : GlobalNPC
	{
		public override void NPCLoot(NPC npc)
		{
			if (Main.rand.Next(50) == 0 && !npc.townNPC && npc.lifeMax > 5 && (npc.value != 0 || (npc.type >= 402 && npc.type <= 429)) && npc.npcSlots != 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagicDice"));
			}
		}
	}
}
