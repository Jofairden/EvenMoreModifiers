using Loot.Core.System;
using Loot.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace Loot
{
	public class EMMWorld : ModWorld
	{
		// The world has not initialized yet, when it is first updated
		public static bool Initialized { get; internal set; }

		public override void Initialize()
		{
			Initialized = false;
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"initialized", Initialized}
			};
		}

		public override void Load(TagCompound tag)
		{
			try
			{
				Initialized = tag.GetBool("initialized");
			}
			catch (Exception e)
			{
				Log4c.Logger.Error("Error on EMMWorld.Load", e);
			}
		}

		public override void PostUpdate()
		{
			if (!Initialized)
			{
				Initialized = true;
				foreach (var chest in Main.chest.Where(chest => chest != null && chest.x > 0 && chest.y > 0))
				{
					WorldGenModifiersPass.GenerateModifiers(null, ModifierContextMethod.FirstLoad, chest.item.Where(x => !x.IsAir), chest);
				}
			}
		}

		// TODO hardmode task, generate better modifiers in new biomes etc.

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			tasks.Add(new WorldGenModifiersPass("EvenMoreModifiers:WorldGenModifiersPass", 1));
		}

		internal sealed class WorldGenModifiersPass : GenPass
		{
			public WorldGenModifiersPass(string name, float loadWeight) : base(name, loadWeight)
			{
			}

			// Attempt rolling modifiers on items
			internal static void GenerateModifiers(GenerationProgress progress, ModifierContextMethod method, IEnumerable<Item> items, object obj = null)
			{
				if (progress != null)
				{
					progress.Message = "Generating modifiers on generated items...";
				}

				foreach (var item in items)
				{
					EMMItem itemInfo = EMMItem.GetItemInfo(item);
					ModifierPool pool = itemInfo.ModifierPool;
					UnifiedRandom rand = Main.rand != null ? Main.rand : WorldGen.genRand != null ? WorldGen.genRand : null;
					if (!itemInfo.HasRolled && pool == null)
					{
						itemInfo.HasRolled = true;

						if (rand != null && rand.NextBool())
						{
							continue;
						}

						ModifierContext ctx = new ModifierContext
						{
							Method = method,
							Item = item
						};

						if (obj is Chest)
						{
							ctx.CustomData = new Dictionary<string, object>
							{
								{"chestData", new Tuple<int, int>(((Chest)obj).x, ((Chest)obj).y)}
							};
						}
						else if (obj is Player)
						{
							ctx.Player = (Player)obj;
						}

						pool = itemInfo.RollNewPool(ctx);
						pool?.ApplyModifiers(item);
					}
				}
			}

			public override void Apply(GenerationProgress progress)
			{
				Initialized = true;
				foreach (var chest in Main.chest.Where(chest => chest != null && chest.x > 0 && chest.y > 0))
				{
					GenerateModifiers(progress, ModifierContextMethod.WorldGeneration, chest.item.Where(x => x != null && !x.IsAir && x.IsModifierRollableItem()), chest);
				}
			}
		}
	}
}
