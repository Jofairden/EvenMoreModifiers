using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Soulforging
{
	internal class LootEssencePlayer : ModPlayer
	{
		public int Essence { get; private set; } = 0;
		public int BonusEssenceGain { get; private set; } = 0;
		public int KillCount { get; set; } = 0;
		public int TopEssence { get; set; } = 1000;

		public int GainEssence(int amount)
		{
			if (!ModContent.GetInstance<LootEssenceWorld>().SoulforgingUnlocked) return 0;

			var player = Main.LocalPlayer;
			var gain = amount + BonusEssenceGain;
			Essence += gain;
			CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), new Color(49, 207, 238), $"+{gain} essence", false, false);
			return Essence;
		}

		public void RecalcTopEssence()
		{
			if (!ModContent.GetInstance<LootEssenceWorld>().SoulforgingUnlocked) return;

			var amount = 1000;
			if (NPC.downedBoss1) amount += 100;
			if (NPC.downedBoss2) amount += 500;
			if (NPC.downedBoss3) amount += 100;
			// TODO do more;
			TopEssence = amount;
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"Essence", Essence},
				{"KillCount", KillCount}
			};
		}

		public override void Load(TagCompound tag)
		{
			Essence = tag.GetInt("Essence");
			KillCount = tag.GetInt("KillCount");
			RecalcTopEssence();
		}
	}
}
