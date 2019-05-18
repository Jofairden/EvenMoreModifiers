using Loot.Api.Ext;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Hacks
{
	/// <summary>
	/// Currently handles armor hack to make armors reforge as accessories
	/// This is needed so that armor can be slotted into the reforge slot
	/// </summary>
	public sealed class TinkererArmorHackModPlayer : ModPlayer
	{
		public override void PostUpdate()
		{
			// The current method of checking if we click inside the tinker slot is fairly ugly
			// But after 2-3 hours of trying things, it seems to be the only way
			// Main.mouseReforge IS NOT available
			// UILinkPointNavigator.Points[304].Position is ZERO
			// vanilla check (flag8) doesn't work either for some reason
			if (Main.mouseLeft && Main.mouseLeftRelease && Main.InReforgeMenu)
			{
				var tinkerPos = new Rectangle(49, 291, 44, 44);
				var mouse = Main.MouseScreen;
				bool isInTinkerSlot = tinkerPos.Intersects(new Rectangle((int) mouse.X, (int) mouse.Y, 20, 20));
				if (isInTinkerSlot)
				{
					// just put in reforge slot
					if (Main.reforgeItem.IsAir && !Main.mouseItem.IsAir && Main.mouseItem.IsArmor())
					{
						var info = LootModItem.GetInfo(Main.mouseItem);
						Main.mouseItem.accessory = true;
						info.JustTinkerModified = true;
					}
					// take out of reforge slot
					else if (!Main.reforgeItem.IsAir && Main.mouseItem.IsAir && Main.reforgeItem.IsArmor())
					{
						var info = LootModItem.GetInfo(Main.reforgeItem);
						Main.reforgeItem.accessory = false;
						info.JustTinkerModified = false;
					}
				}
			}
		}
	}
}
