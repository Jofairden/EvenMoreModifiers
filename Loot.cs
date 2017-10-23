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
	/*
	 * string[] titles = new string[] { "[Uncommon]", "[Rare]", "[Legendary]", "[Transcendent]" };
        string title = "[Common]";
        if (rarity > 3.5f)
        {
            title = titles[3];
        }
        else if (rarity > 2.5f)
        {
            title = titles[2];
        }
        else if (rarity > 1.5f)
        {
            title = titles[1];
        }
        else if (rarity > .5f)
        {
            title = titles[0];
        }
        info.title = title;

		so total  sum of strengths, standard maximum being 1f
		rarity was the sum of the effect magnitudes
		in the original code, a magnitude float was rolled for each affix, which was then stored
		whenever the effect would come into play, it would multiply the base  effect by that magnitude
	 */

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
			Loader.Load();
			Loader.SetupContent();
		}

		public override void Unload()
		{
			Instance = null;
			Loader.Unload();

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
