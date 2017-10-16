using System.IO;
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
		}

		// @todo: probably write our own handler for packets
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{

		}
	}
}
