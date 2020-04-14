using System;
using System.IO;
using Loot.Api.Core;
using Loot.Api.Loaders;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.IO
{
	internal static class ModifierIO
	{
		public const int SAVE_VERSION = 14;

		public static Modifier NetReceive(Item item, BinaryReader reader)
		{
			string type = reader.ReadString();
			string modName = reader.ReadString();
			ModifierProperties properties = ModifierPropertiesIO.NetReceive(item, reader);

			Modifier m = ContentLoader.Modifier.GetContent(modName, type);
			if (m != null)
			{
				m.Properties = m.GetModifierProperties(item).Build();
				m.Properties.Magnitude = properties.Magnitude;
				m.Properties.Power = properties.Power;
				m.NetReceive(item, reader);
				return m;
			}

			throw new Exception($"Modifier _NetReceive error for {modName}");
		}

		public static void NetSend(Modifier modifier, Item item, BinaryWriter writer)
		{
			writer.Write(modifier.GetType().Name);
			writer.Write(modifier.Mod.Name);
			ModifierPropertiesIO.NetSend(item, modifier.Properties, writer);
			modifier.NetSend(item, writer);
		}

		public static Modifier Load(Item item, TagCompound tag)
		{
			string modName = tag.GetString("ModName");

			if (RegistryLoader.Mods.TryGetValue(modName, out var assembly))
			{
				var saveVersion = tag.ContainsKey("SaveVersion") ? tag.GetInt("SaveVersion") : 1;

				// adapt by save version
				if (saveVersion < 14)
				{
					return NullModifier.INSTANCE;
				}

				string modifierTypeName = tag.GetString("Type");
				var modifier = ContentLoader.Modifier.GetContent(modName, modifierTypeName);
				if (modifier == null)
				{
					return NullModifier.INSTANCE;
				}

				var p = ModifierPropertiesIO.Load(item, tag.GetCompound("ModifierProperties"));
				modifier.Properties = modifier.GetModifierProperties(item).Build();
				modifier.Properties.Magnitude = p.Magnitude;
				modifier.Properties.Power = p.Power;
				modifier.Load(item, tag);
				return modifier;

			}

			Loot.Logger.ErrorFormat("There was a load error for modifier, TC: {0}", tag);
			return NullModifier.INSTANCE;
		}

		public static TagCompound Save(Item item, Modifier modifier)
		{
			var tag = new TagCompound
			{
				{"Type", modifier.GetType().Name},
				{"ModName", modifier.Mod.Name},
				{"ModifierProperties", ModifierPropertiesIO.Save(item, modifier.Properties)},
				{"SaveVersion", SAVE_VERSION}
			};
			modifier.Save(item, tag);
			return tag;
		}
	}
}
