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

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            
        }
    }
}
