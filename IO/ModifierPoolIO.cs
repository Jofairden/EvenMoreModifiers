using System;
using System.Collections.Generic;
using System.IO;
using Loot.Api.Core;
using Loot.Api.Loaders;
using Loot.Rarities;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.IO
{
	internal static class ModifierPoolIO
	{
		public const int SAVE_VERSION = 14;

		public static FiniteModifierPool NetReceive(Item item, BinaryReader reader)
		{
			string type = reader.ReadString();
			string modName = reader.ReadString();

			ModifierPool pool = ContentLoader.ModifierPool.GetContent(modName, type);
			if (pool == null)
			{
				throw new Exception($"Modifier _NetReceive error for {modName}");
			}

			int activeModifiersSize = reader.ReadInt32();
			var list = new List<Modifier>();
			for (int i = 0; i < activeModifiersSize; ++i)
			{
				list.Add(ModifierIO.NetReceive(item, reader));
			}

			var finitePool = new FiniteModifierPool(list);
			finitePool.NetReceive(item, reader);
			return finitePool;
		}

		public static void NetSend(FiniteModifierPool pool, Item item, BinaryWriter writer)
		{
			writer.Write(pool.Name);
			writer.Write(pool.Mod.Name);

			writer.Write(pool.Modifiers.Count);
			foreach (var modifier in pool.Modifiers)
			{
				ModifierIO.NetSend(modifier, item, writer);
			}

			pool.NetSend(item, writer);
		}

		public static FiniteModifierPool Load(Item item, TagCompound tag)
		{
			if (tag == null || tag.ContainsKey("EMMErr:PoolNullErr"))
			{
				return NullModifierPool.INSTANCE;
			}

			var saveVersion = tag.ContainsKey("SaveVersion") ? tag.GetInt("SaveVersion") : 1;

			if (saveVersion < 14)
			{
				return NullModifierPool.INSTANCE;
			}

			var list = new List<Modifier>();
			int count = tag.GetAsInt("Count");
			for (int i = 0; i < count; ++i)
			{
				var loaded = ModifierIO.Load(item, tag.Get<TagCompound>($"Modifier{i}"));
				if (loaded != null)
				{
					list.Add(loaded);
				}
			}

			return list.Count > 0 ? new FiniteModifierPool(list) : NullModifierPool.INSTANCE;
		}

		public static TagCompound Save(Item item, FiniteModifierPool pool)
		{
			if (pool == null)
			{
				return new TagCompound { { "EMMErr:PoolNullErr", "ModifierPool was null err" } };
			}

			var tag = new TagCompound
			{
				{"SaveVersion", SAVE_VERSION},
				{"Count", pool.Modifiers.Count}
			};

			for (int i = 0; i < pool.Modifiers.Count; ++i)
			{
				tag.Add($"Modifier{i}", ModifierIO.Save(item, pool.Modifiers[i]));
			}

			pool.Save(item, tag);
			return tag;
		}
	}
}
