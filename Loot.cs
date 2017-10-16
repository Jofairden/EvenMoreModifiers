using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
    public class Loot : Mod
    {
        public Loot()
        {
            Properties = new ModProperties()
            {
                Autoload = true,
                AutoloadGores = true,
                AutoloadSounds = true
            };
        }

		// @todo: probably write our own handler for packets
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            
        }
    }
}
