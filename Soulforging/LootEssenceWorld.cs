using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Soulforging
{
	internal class LootEssenceWorld : ModWorld
	{
		public HashSet<int> UnlockedCubes = new HashSet<int>();
		// TODO for now default unlocked, until souls are implemented
		public bool SoulforgingUnlocked = true;

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
			var items = new Dictionary<string, List<string>>();
			foreach (var type in UnlockedCubes)
			{
				var item = new Item();
				item.SetDefaults(type);
				if (!items.ContainsKey(item.modItem.mod.Name)) items.Add(item.modItem.mod.Name, new List<string>());
				items[item.modItem.mod.Name].Add(item.modItem.Name);
			}
			var tc = new TagCompound();
			foreach (string mod in items.Keys)
			{
				tc.Add(mod, items[mod]);
			}
			return new TagCompound
			{
				{"UnlockedCubes", tc},
				{"SoulforgingUnlocked", SoulforgingUnlocked}
			};
		}

		public override void Load(TagCompound tag)
		{
			foreach (var kvp in tag.GetCompound("UnlockedCubes"))
			{
				var mod = kvp.Key;
				var items = kvp.Value as List<string>;
				items.ForEach(item =>
				{
					UnlockedCubes.Add(ModLoader.GetMod(mod).ItemType(item));
				});
			}
			SoulforgingUnlocked = tag.GetBool("SoulforgingUnlocked");
		}
	}
}
