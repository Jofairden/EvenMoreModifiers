using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace Loot
{
	public class EMMWorld : ModWorld
	{
		// TODO hardmode task, generate better modifiers in new biomes etc.

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			tasks.Add(new WorldGenModifiersPass("EvenMoreModifiers:WorldGenModifiersPass", 1));
		}

		private sealed class WorldGenModifiersPass : GenPass
		{
			public WorldGenModifiersPass(string name, float loadWeight) : base(name, loadWeight)
			{
			}

			public override void Apply(GenerationProgress progress)
			{
				progress.Message = "Generating modifiers on generated items...";

				foreach (Chest chest in Main.chest)
				{
					if (chest == null || chest.x <= 0 || chest.y <= 0)
						continue;

					foreach (Item item in chest.item)
					{
						if (!item.IsAir)
						{
							EMMItem ItemInfo = EMMItem.GetItemInfo(item);
							ModifierPool pool = ItemInfo.ModifierPool;
							if (!ItemInfo.HasRolled && pool == null)
							{
								ModifierContext ctx = new ModifierContext
								{
									Method = ModifierContextMethod.WorldGeneration,
									Item = item,
									CustomData = new Dictionary<string, object>
									{
										{"chestData", new Tuple<int, int>(chest.x, chest.y) }
									}
								};
								pool = ItemInfo.RollNewPool(ctx);
								pool?.ApplyModifiers(item);
							}
						}
					}
				}
			}
		}
	}
}
