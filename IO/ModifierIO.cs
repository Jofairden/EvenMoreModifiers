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
				// If we load a null here, it means a modifier is unloaded
				Modifier m = null;

				var saveVersion = tag.ContainsKey("ModifierSaveVersion") ? tag.GetInt("ModifierSaveVersion") : 1;

				string modifierTypeName = tag.GetString("Type");

				// adapt by save version
				if (saveVersion == 1)
				{
					// in first save version, modifiers were saved by full assembly namespace
					//m = (ModifierPool)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));// we modified saving
					modifierTypeName = modifierTypeName.Substring(modifierTypeName.LastIndexOf('.') + 1);
					m = ContentLoader.Modifier.GetContent(modName, modifierTypeName);
				}
				else if (saveVersion >= 2)
				{
					// from saveVersion 2 and onwards, they are saved by assembly (mod) and type name
					m = ContentLoader.Modifier.GetContent(modName, modifierTypeName);
				}

				if (m != null)
				{
					// saveVersion 1, no longer needed. Type and Mod is already created by new instance
					//m.Type = tag.Get<uint>("ModifierType");
					//m.Mod = ModLoader.GetMod(modname);
					var p = ModifierPropertiesIO.Load(item, tag.GetCompound("ModifierProperties"));
					m.Properties = m.GetModifierProperties(item).Build();
					m.Properties.Magnitude = p.Magnitude;
					m.Properties.Power = p.Power;
					m.Load(item, tag);
					return m;
				}

				return null;
			}

			Loot.Logger.ErrorFormat("There was a load error for modifier, TC: {0}", tag);
			return null;
		}

		public const int SAVE_VERSION = 2;
		public static TagCompound Save(Item item, Modifier modifier)
		{
			var tag = new TagCompound
			{
				{"Type", modifier.GetType().Name},
				//{ "ModifierType", modifier.Type }, //Used to be saved in saveVersion 1
				{"ModName", modifier.Mod.Name},
				{"ModifierProperties", ModifierPropertiesIO.Save(item, modifier.Properties)},
				{"ModifierSaveVersion", SAVE_VERSION}
			};
			modifier.Save(item, tag);
			return tag;
		}
	}
}
