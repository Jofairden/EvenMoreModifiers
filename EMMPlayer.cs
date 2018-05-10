using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Loot
{
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
