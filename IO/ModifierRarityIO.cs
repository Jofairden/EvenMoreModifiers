using System;
using System.IO;
using Loot.Api.Core;
using Loot.Api.Loaders;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.IO
{
	internal static class ModifierRarityIO
	{
		public static ModifierRarity NetReceive(Item item, BinaryReader reader)
		{
			string type = reader.ReadString();
			string modName = reader.ReadString();

			ModifierRarity r = ContentLoader.ModifierRarity.GetContent(modName, type);
			if (r == null)
			{
				throw new Exception($"ModifierRarity _NetReceive error for {modName}");
			}

			r.NetReceive(item, reader);
			return r;
		}

		public static void NetSend(ModifierRarity rarity, Item item, BinaryWriter writer)
		{
			writer.Write(rarity.Name);
			writer.Write(rarity.Mod.Name);
			rarity.NetSend(item, writer);
		}

		public static ModifierRarity Load(Item item, TagCompound tag)
		{
			if (tag == null || tag.ContainsKey("EMMErr:RarityNullErr"))
			{
				return null;
			}

			string modName = tag.GetString("ModName");

			if (RegistryLoader.Mods.TryGetValue(modName, out var assembly))
			{
				// If we load a null here, it means a rarity is unloaded
				ModifierRarity r = null;

				var saveVersion = tag.ContainsKey("ModifierRaritySaveVersion") ? tag.GetInt("ModifierRaritySaveVersion") : 1;

				string rarityTypeName = tag.GetString("Type");

				// adapt by save version
				if (saveVersion == 1)
				{
					// in first save version, modifiers were saved by full assembly namespace
					//m = (ModifierPool)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));// we modified saving
					rarityTypeName = rarityTypeName.Substring(rarityTypeName.LastIndexOf('.') + 1);
					r = ContentLoader.ModifierRarity.GetContent(modName, rarityTypeName);
				}
				else if (saveVersion >= 2)
				{
					// from saveVersion 2 and onwards, they are saved by assembly (mod) and type name
					r = ContentLoader.ModifierRarity.GetContent(modName, rarityTypeName);
				}

				if (r != null)
				{
					//saveVersion 1, no longer needed.Type and Mod is already created by new instance
					//r.Type = tag.Get<uint>("RarityType");
					//r.Mod = ModLoader.GetMod(modname);
					r.Load(item, tag);
					return r;
				}

				return null;
			}

			Loot.Logger.ErrorFormat("There was a load error for modifierrarity, TC: {0}", tag);
			return null;
		}

		public const int SAVE_VERSION = 2;
		public static TagCompound Save(Item item, ModifierRarity rarity)
		{
			if (rarity == null)
			{
				// Should NEVER be null, instead be NullModifierRarity
				return new TagCompound { { "EMMErr:RarityNullErr", "ModifierRarity was null err" } };
			}

			var tag = new TagCompound
			{
				{"Type", rarity.GetType().Name},
				//{ "RarityType", rarity.Type },// saveVersion 1
				{"ModName", rarity.Mod.Name},
				{"ModifierRaritySaveVersion", SAVE_VERSION}
			};
			rarity.Save(item, tag);
			return tag;
		}
	}
}
