using System.IO;
using Terraria.ModLoader;

/*
 * (c) original version by hiccup
 * reworked and maintained by jofairden
 * for tmodloader
 */

namespace Loot
{
	public class Loot : Mod
	{
		internal static Loot Instance;

#if DEBUG
		public override string Name => "Loot";
#endif

		public Loot()
		{
			Properties = new ModProperties
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load()
		{
			Instance = this;

			//(string Name, string test) variable = ("Compiled with", "C#7");

			EMMLoader.Initialize();
			EMMLoader.Load();

			EMMLoader.RegisterMod(this);
			EMMLoader.SetupContent(this);
		}

		public override void Unload()
		{
			Instance = null;
			EMMLoader.Unload();

			// TODO causes trouble in unload?
			// @todo this is not a feature of tml
			// Attempt to unload our static variables
			//Stack<Type> typesToProcess = new Stack<Type>(this.Code.GetTypes());
			//while (typesToProcess.Count > 0)
			//{
			//    Type type = typesToProcess.Pop();
			//    foreach (FieldInfo info in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			//    {
			//        info.SetValue(null, info.FieldType.IsValueType ? Activator.CreateInstance(info.FieldType) : null);
			//    }
			//    foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			//    {
			//        typesToProcess.Push(nestedType);
			//    }
			//}
		}

		// @todo: probably write our own handler for packets
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{

		}
	}
}
