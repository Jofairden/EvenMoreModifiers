using Terraria.ModLoader;

namespace Loot.Soulforging
{
	internal class LootEssencePlayer : ModPlayer
	{
		public int Essence { get; private set; } = 0;
		public int BonusEssenceGain { get; private set; } = 0;
		public int KillCount { get; set; } = 0;

		public int GainEssence(int amount)
		{
			Essence += amount + BonusEssenceGain;
			return Essence;
		}
	}
}
