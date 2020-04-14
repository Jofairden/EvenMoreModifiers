using System;
using System.IO;
using Loot.Api.Core;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.IO
{
	internal static class ModifierPropertiesIO
	{
		public const int SAVE_VERSION = 14;

		public static ModifierProperties NetReceive(Item item, BinaryReader reader)
		{
			var p = new ModifierProperties
			{
				Magnitude = reader.ReadSingle(),
				Power = reader.ReadSingle()
			};
			p.NetReceive(item, reader);
			return p;
		}

		public static void NetSend(Item item, ModifierProperties properties, BinaryWriter writer)
		{
			writer.Write(properties.Magnitude);
			writer.Write(properties.Power);
			properties.NetSend(item, writer);
		}

		public static ModifierProperties Load(Item item, TagCompound tag)
		{
			ModifierProperties prop;
			try
			{
				prop = new ModifierProperties
				{
					Magnitude = tag.GetFloat("Magnitude"),
					Power = tag.GetFloat("Power")
				};
			}
			catch (Exception)
			{
				prop = new ModifierProperties().RollMagnitudeAndPower();
			}

			prop.Load(item, tag);
			return prop;
		}

		public static TagCompound Save(Item item, ModifierProperties properties)
		{
			var tc = new TagCompound
			{
				{"Magnitude", properties.Magnitude},
				{"Power", properties.Power},
				{"SaveVersion", SAVE_VERSION}
			};
			properties.Save(item, tc);
			return tc;
		}
	}
}
