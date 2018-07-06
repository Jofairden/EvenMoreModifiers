using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// Currently handles armor hack to make armors reforge as accessories
	/// </summary>
	public class EMMPlayer : ModPlayer
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
				bool isInTinkerSlot = tinkerPos.Intersects(new Rectangle((int)mouse.X, (int)mouse.Y, 20, 20));
				if (isInTinkerSlot)
				{
					// just put in reforge slot
					if (Main.reforgeItem.IsAir && !Main.mouseItem.IsAir && Main.mouseItem.IsArmor())
					{
						var info = EMMItem.GetItemInfo(Main.mouseItem);
						Main.mouseItem.accessory = true;
						info.JustTinkerModified = true;
					}
					// take out of reforge slot
					else if (!Main.reforgeItem.IsAir && Main.mouseItem.IsAir && Main.reforgeItem.IsArmor())
					{
						var info = EMMItem.GetItemInfo(Main.reforgeItem);
						Main.reforgeItem.accessory = false;
						info.JustTinkerModified = false;
					}
				}
			}
		}
	}

	// tinker slot hack
	//public class EMMPlayer : ModPlayer
	//{
	//	internal bool JustMenusClicked;
	//	internal bool IsYetForced;
	//	internal int? LastReforgeId = null;

	//	internal void Reset(ref Item item)
	//	{
	//		var refItem = new Item();
	//		refItem.netDefaults(item.netID);
	//		item = refItem.CloneWithModdedDataFrom(item);
	//	}

	//	private bool InMenusWithMouseItem => Main.mouseItem != null
	//									&& !Main.mouseItem.IsAir
	//									&& Main.InReforgeMenu;

	//	private bool InMenusWithReforgeItem => Main.reforgeItem != null
	//										   && !Main.reforgeItem.IsAir
	//										   && Main.InReforgeMenu;

	//	public override void PostUpdate()
	//	{
	//		if (JustMenusClicked)
	//		{
	//			// just put in reforge slot
	//			if (Main.reforgeItem.IsAir && !Main.mouseItem.IsAir && IsYetForced)
	//			{
	//				JustMenusClicked = false;
	//				LastReforgeId = Main.mouseItem?.netID;
	//				//Reset(ref Main.mouseItem);
	//			}

	//			// just taken from reforge slot
	//			else if (Main.mouseItem.IsAir && !Main.reforgeItem.IsAir)
	//			{
	//				JustMenusClicked = false;
	//				IsYetForced = false;
	//				LastReforgeId = null;
	//				//Reset(ref Main.reforgeItem);
	//			}

	//		}
	//		// in reforge menus with mouse item
	//		else if (InMenusWithMouseItem && !IsYetForced)
	//		{
	//			ForceReforgeMode(Main.mouseItem);
	//		}
	//	}

	//	public override void ProcessTriggers(TriggersSet triggersSet)
	//	{

	//		if (Main.mouseLeftRelease && Main.mouseLeft && InMenusWithMouseItem || InMenusWithReforgeItem)
	//		{
	//			JustMenusClicked = true;
	//		}

	//	}

	//	private void ForceReforgeMode(Item item)
	//	{
	//		var info = EMMItem.GetItemInfo(item);
	//		if ((info.CustomReforgeMode & CustomReforgeMode.ForceAccessory) == CustomReforgeMode.ForceAccessory
	//			&& item.prefix != mod.PrefixType<ItemHackForceAccessory>())
	//		{

	//			item.Prefix(-3);
	//			IsYetForced = true;
	//		}

	//		if ((info.CustomReforgeMode & CustomReforgeMode.ForceWeapon) == CustomReforgeMode.ForceWeapon
	//			&& item.prefix != mod.PrefixType<ItemHackForceWeapon>())
	//		{

	//			item.Prefix(-3);
	//			IsYetForced = true;
	//		}

	//		if ((info.CustomReforgeMode & CustomReforgeMode.ForceSimulate) == CustomReforgeMode.ForceSimulate
	//			|| (info.CustomReforgeMode & CustomReforgeMode.Custom) == CustomReforgeMode.Custom
	//			&& item.prefix != mod.PrefixType<ItemHackForceSimulate>())
	//		{

	//			item.Prefix(-3);
	//			IsYetForced = true;
	//		}
	//	}
	//}
}
