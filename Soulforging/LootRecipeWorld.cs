using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Soulforging
{
	internal class LootRecipeWorld : ModWorld
	{
		public HashSet<int> UnlockedCubes = new HashSet<int>();

		public void UnlockCube(int type)
		{
			UnlockedCubes.Add(type);
		}

		public bool IsCubeUnlocked(int type)
		{
			return UnlockedCubes.Contains(type);
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"UnlockedCubes", UnlockedCubes.ToList()}
			};
		}

		public override void Load(TagCompound tag)
		{
			UnlockedCubes = new HashSet<int>(tag.GetList<int>("UnlockedCubes"));
		}
	}
}
