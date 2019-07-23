using System;
using System.IO;
using Loot.Api.Core;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.IO
{
	internal static class ModifierPropertiesIO
	{
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
				// Something was wrong with the TC, roll new values
				prop = new ModifierProperties().RollMagnitudeAndPower();
			}

			prop.Load(item, tag);
			return prop;
		}

		public const int SAVE_VERSION = 1;
		public static TagCompound Save(Item item, ModifierProperties properties)
		{
			var tc = new TagCompound
			{
				{"Magnitude", properties.Magnitude},
				{"Power", properties.Power},
				{"ModifierPropertiesSaveVersion", SAVE_VERSION}
			};
			properties.Save(item, tc);
			return tc;
		}
	}
}
