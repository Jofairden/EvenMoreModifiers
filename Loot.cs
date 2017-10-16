using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Loot.Modifiers;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	public class Loot : Mod
	{
		internal static Loot Instance;

		public Loot()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load()
		{
			Instance = this;
			ModifierLoader.Load();
			ModifierLoader.SetupContent();
		}

		public override void Unload()
		{
			Instance = null;
			ModifierLoader.Unload();

			// @todo this is not a feature of tml
			// Attempt to unload our static variables
			Stack<Type> typesToProcess = new Stack<Type>(this.Code.GetTypes());
			while (typesToProcess.Count > 0)
			{
				Type type = typesToProcess.Pop();
				foreach (FieldInfo info in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					info.SetValue(null, info.FieldType.IsValueType ? Activator.CreateInstance(info.FieldType) : null);
				}
				foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					typesToProcess.Push(nestedType);
				}
			}
		}

		// @todo: probably write our own handler for packets
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{

		}
	}
}
