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
		public const int SAVE_VERSION = 14;

		public static ModifierRarity NetReceive(Item item, BinaryReader reader)
		{
			string type = reader.ReadString();
			string modName = reader.ReadString();

			ModifierRarity rarity = ContentLoader.ModifierRarity.GetContent(modName, type);
			if (rarity == null)
			{
				throw new Exception($"ModifierRarity _NetReceive error for {modName}");
			}

			rarity.NetReceive(item, reader);
			return rarity;
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
				return NullModifierRarity.INSTANCE;
			}

			string modName = tag.GetString("ModName");

			if (RegistryLoader.Mods.TryGetValue(modName, out var assembly))
			{
				var saveVersion = tag.ContainsKey("SaveVersion") ? tag.GetInt("SaveVersion") : 1;

				if (saveVersion < 14)
				{
					return NullModifierRarity.INSTANCE;
				}

				string rarityTypeName = tag.GetString("Type");
				var rarity = ContentLoader.ModifierRarity.GetContent(modName, rarityTypeName);
				if (rarity == null)
				{
					return NullModifierRarity.INSTANCE;
				}

				rarity.Load(item, tag);
				return rarity;
			}

			Loot.Logger.ErrorFormat("There was a load error for modifierrarity, TC: {0}", tag);
			return NullModifierRarity.INSTANCE;
		}

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
				{"ModName", rarity.Mod.Name},
				{"SaveVersion", SAVE_VERSION}
			};
			rarity.Save(item, tag);
			return tag;
		}
	}
}
