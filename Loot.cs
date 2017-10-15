using System.IO;
using Terraria;
using Terraria.ModLoader;
using Loot.Items;

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
            string msgType = reader.ReadString();
            if (msgType.Equals("Prefix"))
            {
                Item item = Main.item[reader.ReadInt32()];
                gItem.addPrefix(item, reader.ReadInt32(), (float)reader.ReadDouble(), reader.ReadInt32());
            }
            else if (msgType.Equals("SyncRolled"))
            {
                Item item = Main.item[reader.ReadInt32()];
                EMMItem info = item.GetGlobalItem<EMMItem>();
                info.alreadyRolled = reader.ReadBoolean();
                info.title = reader.ReadString();
            }
            else if (msgType.Equals("Reset"))
            {
                Item item = Main.item[reader.ReadInt32()];
                EMMItem info = item.GetGlobalItem<EMMItem>();
                info.alreadyRolled = false;
                info.prefixIDs = new int[] { -1, -1, -1, -1 };
                info.prefixMagnitude = new int[] { -1, -1, -1, -1 };
                info.prefixMagnitudePercent = new double[] { -1, -1, -1, -1 };
                info.title = "";
            }
        }
    }
}
