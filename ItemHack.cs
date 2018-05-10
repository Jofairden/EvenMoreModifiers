using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Modifiers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Loot
{
	//public class ItemHack : GlobalItem
	//{
		//internal static void ModifyItemCustomReforce(EMMItem info, Item item, bool resetInstead = false)
		//{
		//	if (info.CustomReforgeMode.HasFlag(CustomReforgeMode.ForceWeapon))
		//	{
		//		if (resetInstead)
		//		{
		//			var baseItem = new Item();
		//			baseItem.netDefaults(item.netID);
		//			item.ranged = baseItem.ranged;
		//			item.magic = baseItem.magic;
		//			item.thrown = baseItem.thrown;
		//			item.summon = baseItem.summon;
		//			item.melee = baseItem.melee;
		//			item.damage = baseItem.damage;
		//		}
		//		else
		//		{
		//			item.ranged = false;
		//			item.magic = false;
		//			item.thrown = false;
		//			item.summon = false;
		//			item.melee = true;
		//			if (item.damage <= 0)
		//				item.damage = 1;
		//		}

		//	}

		//	if (info.CustomReforgeMode.HasFlag(CustomReforgeMode.ForceAccessory))
		//	{
		//		if (resetInstead)
		//		{
		//			var baseItem = new Item();
		//			baseItem.netDefaults(item.netID);
		//			item.accessory = baseItem.accessory;
		//		}
		//		else
		//		{
		//			item.accessory = true;
		//		}
		//	}

		//	if (info.CustomReforgeMode.HasFlag(CustomReforgeMode.ForceSimulate))
		//	{
		//		if (resetInstead)
		//		{
		//			var baseItem = new Item();
		//			baseItem.netDefaults(item.netID);
		//			item.accessory = baseItem.accessory;
		//		}
		//		else
		//		{
		//			item.accessory = true;
		//		}
		//	}
		//}

		//public override int ChoosePrefix(Item item, UnifiedRandom rand)
		//{
		//	var mplr = Main.LocalPlayer.GetModPlayer<EMMPlayer>();
		//	var info = EMMItem.GetItemInfo(item);

		//	if (!mplr.IsYetForced)
		//	{
		//		if (info.CustomReforgeMode.HasFlag(CustomReforgeMode.ForceWeapon))
		//			return mod.PrefixType<ItemHackForceWeapon>();

		//		if (info.CustomReforgeMode.HasFlag(CustomReforgeMode.ForceAccessory))
		//			return mod.PrefixType<ItemHackForceAccessory>();

		//		if (info.CustomReforgeMode.HasFlag(CustomReforgeMode.ForceSimulate))
		//			return mod.PrefixType<ItemHackForceSimulate>();
		//	}
		//	else
		//	{
		//		ModifyItemCustomReforce(info, item);
		//	}

		//	return -1;
		//}

		//public override void UpdateInventory(Item item, Player player)
		//{
		//	var mplr = player.GetModPlayer<EMMPlayer>();
		//	if (mplr.IsYetForced && mplr.LastReforgeId.HasValue && mplr.LastReforgeId == item.netID)
		//	{
		//		mplr.Reset(ref item);
		//		mplr.IsYetForced = false;
		//		mplr.JustMenusClicked = false;
		//		mplr.LastReforgeId = null;
		//	}
		//}

		//public override void UpdateEquip(Item item, Player player)
		//{
		//	var mplr = player.GetModPlayer<EMMPlayer>();
		//	if (mplr.IsYetForced && mplr.LastReforgeId.HasValue && mplr.LastReforgeId == item.netID)
		//	{
		//		mplr.Reset(ref item);
		//		mplr.IsYetForced = false;
		//		mplr.JustMenusClicked = false;
		//		mplr.LastReforgeId = null;
		//	}
		//}

		//public override bool NewPreReforge(Item item)
		//{
		//	var mplr = Main.LocalPlayer.GetModPlayer<EMMPlayer>();
		//	var info = EMMItem.GetItemInfo(Main.reforgeItem);

		//	bool canApplyDiscount = true;
		//	int price = Main.reforgeItem.value;

		//	if (ItemLoader.ReforgePrice(Main.reforgeItem, ref price, ref canApplyDiscount))
		//	{
		//		if (canApplyDiscount && Main.player[Main.myPlayer].discount)
		//		{
		//			price = (int)((double)price * 0.8);
		//		}
		//		price /= 3;
		//	}

		//	if (!info.CustomReforgeMode.HasFlag(CustomReforgeMode.Vanilla))
		//	{
		//		Main.LocalPlayer.BuyItem(price, -1);
		//		bool favorited = Main.reforgeItem.favorited;
		//		int stack = Main.reforgeItem.stack;
		//		Item reforgeItem = new Item();
		//		reforgeItem.netDefaults(Main.reforgeItem.netID);
		//		Main.reforgeItem = reforgeItem.CloneWithModdedDataFrom(Main.reforgeItem);
		//		mplr.IsYetForced = true;
		//		mplr.LastReforgeId = Main.reforgeItem.netID;
		//		Main.reforgeItem.Prefix(-2);
		//		if (Main.reforgeItem.modItem != null && info.CustomReforgeMode.HasFlag(CustomReforgeMode.Custom))
		//		{
		//			var EMMCustomReforge = Main.reforgeItem.modItem.GetType().GetMethod("EMMCustomReforge");
		//			EMMCustomReforge?.Invoke(Main.reforgeItem.modItem, null);
		//		}
		//		mplr.IsYetForced = false;
		//		Main.reforgeItem.position.X = Main.LocalPlayer.Center.X;
		//		Main.reforgeItem.position.Y = Main.LocalPlayer.Center.Y;
		//		Main.reforgeItem.favorited = favorited;
		//		Main.reforgeItem.stack = stack;
		//		item = item.CloneWithModdedDataFrom(Main.reforgeItem);
		//		ItemLoader.PostReforge(Main.reforgeItem);
		//		ItemText.NewText(Main.reforgeItem, Main.reforgeItem.stack, true, false);
		//		Main.PlaySound(SoundID.Item37, -1, -1);

		//		return false;
		//	}

		//	return true;
		//}
	//}
}
