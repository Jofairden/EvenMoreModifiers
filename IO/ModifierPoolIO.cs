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
		public static ModifierPool NetReceive(Item item, BinaryReader reader)
		{
			string type = reader.ReadString();
			string modName = reader.ReadString();

			ModifierPool p = ContentLoader.ModifierPool.GetContent(modName, type);
			if (p == null)
			{
				throw new Exception($"Modifier _NetReceive error for {modName}");
			}

			int activeModifiersSize = reader.ReadInt32();
			var list = new List<Modifier>();
			for (int i = 0; i < activeModifiersSize; ++i)
			{
				list.Add(ModifierIO.NetReceive(item, reader));
			}

			p.ActiveModifiers = list.ToArray();
			p.NetReceive(item, reader);
			return p;
		}

		public static void NetSend(ModifierPool modifierPool, Item item, BinaryWriter writer)
		{
			writer.Write(modifierPool.Name);
			writer.Write(modifierPool.Mod.Name);

			writer.Write(modifierPool.ActiveModifiers.Length);
			for (int i = 0; i < modifierPool.ActiveModifiers.Length; ++i)
			{
				ModifierIO.NetSend(modifierPool.ActiveModifiers[i], item, writer);
			}

			modifierPool.NetSend(item, writer);
		}

		public static ModifierPool Load(Item item, TagCompound tag)
		{
			if (tag == null || tag.ContainsKey("EMMErr:PoolNullErr"))
			{
				return null;
			}

			string modName = tag.GetString("ModName");

			if (RegistryLoader.Mods.TryGetValue(modName, out var assembly))
			{
				// If we manage to load null here, that means some pool got unloaded
				ModifierPool p = null;
				var saveVersion = tag.ContainsKey("ModifierPoolSaveVersion") ? tag.GetInt("ModifierPoolSaveVersion") : 1;

				string poolTypeName = tag.GetString("Type");

				// adapt by save version
				if (saveVersion == 1)
				{
					// in first save version, modifiers were saved by full assembly namespace
					//m = (ModifierPool)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));// we modified saving
					poolTypeName = poolTypeName.Substring(poolTypeName.LastIndexOf('.') + 1);
					p = ContentLoader.ModifierPool.GetContent(modName, poolTypeName);
				}
				else if (saveVersion >= 2)
				{
					// from saveVersion 2 and onwards, they are saved by assembly (mod) and type name
					p = ContentLoader.ModifierPool.GetContent(modName, poolTypeName);
				}

				// if we have a pool
				if (p != null)
				{
					// saveVersion 1, no longer needed. Type and Mod is already created by new instance
					//m.Type = tag.Get<uint>("ModifierType");
					// m.Mod = ModLoader.GetMod(modname);
					// preload rarity
					if (saveVersion < 3)
					{
						// Since save version 3, the rarity is no longer saved on the pool, rather on the item itself
						// however, it was saved on the pool in earlier versions, so we need to catch it here.
						ModifierRarity preloadRarity = ModifierRarityIO.Load(item, tag.GetCompound("Rarity"));

						LootModItem.GetInfo(item).ModifierRarity =
							preloadRarity ?? ContentLoader.ModifierRarity.GetContent(typeof(CommonRarity));
					}

					int activeModifiers = tag.GetAsInt("ActiveModifiers");
					if (activeModifiers > 0)
					{
						var list = new List<Modifier>();
						for (int i = 0; i < activeModifiers; ++i)
						{
							// preload to take unloaded modifiers into account
							var loaded = ModifierIO.Load(item, tag.Get<TagCompound>($"ActiveModifier{i}"));
							if (loaded != null)
							{
								list.Add(loaded);
							}
						}

						p.ActiveModifiers = list.ToArray();
					}

					p.Load(item, tag);
					return p;
				}

				return null;
			}

			Loot.Logger.ErrorFormat("There was a load error for modifierpool, TC: {0}", tag);
			return null;
		}

		public const int SAVE_VERSION = 3;
		public static TagCompound Save(Item item, ModifierPool modifierPool)
		{
			if (modifierPool == null)
			{
				// Should NEVER be null, instead should be NullModifierPool
				return new TagCompound { { "EMMErr:PoolNullErr", "ModifierPool was null err" } };
			}

			var tag = new TagCompound
			{
				{"Type", modifierPool.GetType().Name},
				//{ "ModifierType", modifierPool.Type }, //saveVersion 1
				{"ModName", modifierPool.Mod.Name},
				{"ModifierPoolSaveVersion", SAVE_VERSION},
				{"ActiveModifiers", modifierPool.ActiveModifiers.Length}
			};

			for (int i = 0; i < modifierPool.ActiveModifiers.Length; ++i)
			{
				tag.Add($"ActiveModifier{i}", ModifierIO.Save(item, modifierPool.ActiveModifiers[i]));
			}

			modifierPool.Save(item, tag);
			return tag;
		}
	}
}
