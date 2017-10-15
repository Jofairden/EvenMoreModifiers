using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Items
{
	public class EMMItem : GlobalItem
	{
		public bool alreadyRolled = false;
		public string title = "";
		public int[] prefixIDs = new int[] { -1, -1, -1, -1 };
		public int[] prefixMagnitude = new int[] { -1, -1, -1, -1 };
		public double[] prefixMagnitudePercent = new double[] { -1, -1, -1, -1 };
		public override bool CloneNewInstances => true;
		public override bool InstancePerEntity => true;
	}

	public class gItem : GlobalItem
	{
		public override GlobalItem Clone(Item item, Item itemClone)
		{
			var source = item.GetGlobalItem<EMMItem>();
			var destination = itemClone.GetGlobalItem<EMMItem>();
			destination.alreadyRolled = source.alreadyRolled;
			destination.title = source.title;
			for (int i = 0; i < 4; i++)
			{
				if (source.prefixMagnitude[i] > 1)
				{
					//Main.NewText("source " + i + " " + source.prefixMagnitude[i]);
					//Main.NewText("destination " + i + " " + destination.prefixMagnitude[i]);
				}
			}
			/*
            destination.prefixIDs = (int[])source.prefixIDs.Clone();
            destination.prefixMagnitude = (int[])source.prefixMagnitude.Clone();
            destination.prefixMagnitudePercent = (double[])source.prefixMagnitudePercent.Clone();
            */
			destination.prefixIDs = new int[4];
			destination.prefixMagnitude = new int[4];
			destination.prefixMagnitudePercent = new double[4];
			for (int i = 0; i < 4; i++)
			{
				destination.prefixIDs[i] = source.prefixIDs[i];
				destination.prefixMagnitude[i] = source.prefixMagnitude[i];
				destination.prefixMagnitudePercent[i] = source.prefixMagnitudePercent[i];
			}
			for (int i = 0; i < 4; i++)
			{
				if (source.prefixMagnitude[i] > 1)
				{
					//Main.NewText("POSTsource " + i + " " + source.prefixMagnitude[i]);
					//Main.NewText("POSTdestination " + i + " " + destination.prefixMagnitude[i]);
				}
			}
			return destination;
		}


		public override bool OnPickup(Item item, Player player)
		{
			rollWithLuck(item, player);
			return true;
		}
		public override void OnCraft(Item item, Recipe recipe)
		{
			rollWithLuck(item, Main.player[Main.myPlayer]);
		}
		public override void PostReforge(Item item)
		{
			resetInfo(item);
			item.stack--;
			rollWithLuck(item, Main.player[Main.myPlayer]);
		}
		public void roll(Item item)
		{
			EMMItem info = item.GetGlobalItem<EMMItem>();
			if (item.type == 0)
			{
				return;
			}
			float rarity = 0;
			if (item.consumable || info.alreadyRolled || item.ammo > 0)
			{
				return;
			}
			info.alreadyRolled = true;
			int numPrefixes = 0;
			for (int i = 0; i < 4; i++)
			{
				if (Main.rand.Next(0, 2) == 0)
				{
					numPrefixes++;
				}
				else
				{
					break;
				}
			}
			while (numPrefixes > 0)
			{
				int type = -1;
				if (item.accessory || item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0)
				{
					type = Main.rand.Next(0, 27);
					if (Main.rand.Next(0, 3) != 0 && type == 19)//rare types, 2/3 chance to reroll
					{
						type = Main.rand.Next(0, 20);
					}
				}
				else if (item.damage > 0)
				{
					type = Main.rand.Next(0, 19);
					#region type combatibility checks
					if (type == 7 && item.mana > 0)
					{
						type = -1;
					}
					if (type == 6 && item.useAmmo == 0)
					{
						type = -1;
					}
					if (type == 5 && item.mana == 0)
					{
						type = -1;
					}
					if (type == 4 && item.shootSpeed == 0)
					{
						type = -1;
					}
					if (type == 3 && item.knockBack == 0)
					{
						type = -1;
					}
					#endregion
				}
				else
				{
					break;
				}
				for (int i = 0; i < 4; i++)
				{
					if (info.prefixIDs[i] == type)
					{
						type = -1;
						break;
					}
				}
				if (type != -1)//if a type couldn't be given for some reason, it'll just try again
				{
					numPrefixes--;
					float scalar = (float)Math.Max(.1, Main.rand.NextDouble() - Main.rand.NextDouble() / 5);
					rarity += scalar;
					addPrefix(item, type, scalar, numPrefixes);
					if (Main.netMode == 1)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write("Prefix");
						netMessage.Write(item.whoAmI);
						netMessage.Write(type);
						netMessage.Write((double)scalar);
						netMessage.Write(numPrefixes);
						netMessage.Send();
					}
				}
			}
			item.value = (int)(item.value * (1 + rarity * rarity / 2));
			//modify item name to reflect value
			string[] titles = new string[] { "[Uncommon]", "[Rare]", "[Legendary]", "[Transcendent]" };
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
			if (Main.netMode == 1)
			{
				var netMessage2 = mod.GetPacket();
				netMessage2.Write("SyncRolled");
				netMessage2.Write(item.whoAmI);
				netMessage2.Write(info.alreadyRolled);
				netMessage2.Write(info.title);
				netMessage2.Send();
			}
		}
		public void rollWithLuck(Item item, Player player)
		{
			EMMItem info = item.GetGlobalItem<EMMItem>();
			for (int i = 0; i < 4; i++)
			{
				//Main.NewText("pre: " + info.prefixMagnitude[i]);
			}
			if (item.type == 0)
			{
				return;
			}
			MPlayer mplayer = (MPlayer)(player.GetModPlayer(mod, "MPlayer"));
			float rarity = 0;
			if (item.consumable || info.alreadyRolled || item.ammo > 0 || item.maxStack > 1)
			{
				return;
			}
			info.alreadyRolled = true;
			int numPrefixes = 0;
			for (int i = 0; i < 4; i++)
			{
				if (Main.rand.Next(300) <= 150 + mplayer.luck)
				{
					numPrefixes++;
				}
				else
				{
					break;
				}
			}
			if (numPrefixes == 0 && Main.rand.Next(3) == 0)
			{
				numPrefixes++;
			}
			while (numPrefixes > 0)
			{
				int type = -1;
				if (item.accessory || item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0)
				{
					type = Main.rand.Next(0, 27);
					if (Main.rand.Next(0, 3) != 0 && type == 19)//rare types, 2/3 chance to reroll
					{
						type = Main.rand.Next(0, 27);
					}
				}
				else if (item.damage > 0)
				{
					type = Main.rand.Next(0, 19);
					#region type combatibility checks
					if (type == 7 && item.mana > 0)
					{
						type = -1;
					}
					if (type == 6 && item.useAmmo == 0)
					{
						type = -1;
					}
					if (type == 5 && item.mana == 0)
					{
						type = -1;
					}
					if (type == 4 && item.shootSpeed == 0)
					{
						type = -1;
					}
					if (type == 3 && item.knockBack == 0)
					{
						type = -1;
					}
					#endregion
				}
				else
				{
					break;
				}
				for (int i = 0; i < 4; i++)
				{
					if (info.prefixIDs[i] == type)
					{
						type = -1;
						break;
					}
				}
				if (type != -1)//if a type couldn't be given for some reason, it'll just try again
				{
					numPrefixes--;
					float scalar = (float)Math.Max(.1, Main.rand.NextDouble() - Main.rand.NextDouble() / 5);
					scalar += (float)(mplayer.luck * (Math.Max(0, Main.rand.NextDouble() - Main.rand.NextDouble() / 4)) / 100);
					if (scalar > 1.2f)
					{
						scalar = 1.2f;
					}
					rarity += scalar;
					addPrefix(item, type, scalar, numPrefixes);
					if (Main.netMode == 1)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write("Prefix");
						netMessage.Write(item.whoAmI);
						netMessage.Write(type);
						netMessage.Write((double)scalar);
						netMessage.Write(numPrefixes);
						netMessage.Send();
					}
				}
			}
			for (int i = 0; i < 4; i++)
			{
				Main.NewText("post: " + info.prefixMagnitude[i]);
			}
			item.value = (int)(item.value * (1 + rarity * rarity / 2));
			//modify item name to reflect value
			string[] titles = new string[] { "[Uncommon]", "[Rare]", "[Legendary]", "[Transcendent]" };
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
			if (Main.netMode == 1)
			{
				var netMessage2 = mod.GetPacket();
				netMessage2.Write("SyncRolled");
				netMessage2.Write(item.whoAmI);
				netMessage2.Write(info.alreadyRolled);
				netMessage2.Write(info.title);
				netMessage2.Send();
			}
		}
		public static void addPrefix(Item item, int ID, float scalar, int slot)
		{
			if (ID == -1 || scalar <= 0)
			{
				return;
			}
			Main.NewText("adding prefix");
			EMMItem info = item.GetGlobalItem<EMMItem>();
			int magnitude = 0;
			int storedID = ID;
			float storedScalar = scalar;
			#region equips
			if (item.accessory || item.headSlot > 0 || item.bodySlot > 0 || item.legSlot > 0)
			{
				if (item.accessory)
				{
					scalar *= .6f;
					storedScalar *= .6f;
				}
				if (ID < 9)
				{
					ID = 0;//first 9 all share the same code here
				}
				switch (ID)
				{
					case 0://class specific damage/crit
						magnitude = (int)Math.Round(scalar * 8);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 9://light
						magnitude = (int)Math.Round(scalar * 5);
						if (magnitude < 1)
						{
							magnitude = 1;
						}
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 10: goto case 11;
					case 11://max mana/health
						magnitude = (int)Math.Round(scalar * 20);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 12://crit damage
						magnitude = (int)Math.Round(scalar * 15);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 13://wing time
						magnitude = (int)Math.Round(scalar * 60);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 14://luck
						magnitude = (int)Math.Round(scalar * 12);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 15://thorns
						magnitude = (int)Math.Round(scalar * 50);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 16://mining
						magnitude = (int)Math.Round(scalar * 10);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 17://immune time
						magnitude = (int)Math.Round(scalar * 30);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 18://dodge, hard cap 40%
						magnitude = (int)Math.Round(scalar * 5);
						if (magnitude < 1)
						{
							magnitude = 1;
						}
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 19://knockback immunity
						magnitude = 1;
						storedScalar = item.accessory ? .6f : 1f;
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 20://fishing skill
						magnitude = (int)Math.Round(scalar * 8);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 21://damage to max life
						magnitude = (int)Math.Round(scalar * 20);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 22://miracle
						magnitude = (int)Math.Round(scalar * 15);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 23://mana shield
						magnitude = (int)Math.Round(scalar * 6);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 24://defense
						magnitude = (int)Math.Round(scalar * 8);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 25://life regen
						magnitude = (int)Math.Round(scalar * 45);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 26://movespeed
						magnitude = (int)Math.Round(scalar * 10);
						info.prefixMagnitude[slot] = magnitude;
						break;

					default: break;
				}
			}
			#endregion
			#region weapon
			else if (item.damage > 0)
			{
				if (ID > 13 && ID <= 17)
				{
					ID = 13;
				}
				switch (ID)
				{
					case 0:
						magnitude = (int)Math.Round(scalar * 10);
						info.prefixMagnitude[slot] = magnitude;
						item.damage = (int)(item.damage * (1 + magnitude / 100f));
						break;
					case 1:
						magnitude = (int)Math.Round(scalar * 10);
						info.prefixMagnitude[slot] = magnitude;
						item.crit += magnitude;
						break;
					case 2:
						magnitude = (int)Math.Round(scalar * 10);
						info.prefixMagnitude[slot] = magnitude;
						item.useTime = (int)(item.useTime * (100 - magnitude) / 100);
						item.useAnimation = (int)(item.useAnimation * (100 - magnitude) / 100.0);
						break;
					case 3:
						magnitude = (int)Math.Round(scalar * 20);
						info.prefixMagnitude[slot] = magnitude;
						item.knockBack *= (1 + magnitude / 100f);
						break;
					case 4:
						magnitude = (int)Math.Round(scalar * 20);
						info.prefixMagnitude[slot] = magnitude;
						item.shootSpeed *= (1 + magnitude / 100f);
						break;
					case 5:
						magnitude = (int)Math.Round(scalar * 15);
						info.prefixMagnitude[slot] = magnitude;
						item.mana = (int)(item.mana * (100 - magnitude) / 100.0);
						break;
					case 6://ammo cost
						magnitude = (int)Math.Round(scalar * 20);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 7://add mana cost, damage
						magnitude = (int)Math.Round(scalar * 15) + 15;
						item.mana = Math.Max((int)(25 * (item.useTime / 60.0)), 1);
						item.damage = (int)(item.damage * (1 + magnitude / 100f));
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 8://+damage day
						magnitude = (int)Math.Round(scalar * 15);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 9://+damage night
						magnitude = (int)Math.Round(scalar * 15);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 10://cursed
						magnitude = (int)Math.Round(scalar * 15) + 15;
						info.prefixMagnitude[slot] = magnitude;
						item.damage = (int)(item.damage * (1 + magnitude / 100f));
						break;
					case 11://+damage missing health
						magnitude = (int)Math.Round(scalar * 5);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 12://+damage velocity
						magnitude = (int)Math.Round(scalar * 7);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 13://debuff chance while held: poison, on fire, frostburn, confusion, inferno
						magnitude = (int)Math.Round(scalar * 50);
						info.prefixMagnitude[slot] = magnitude;
						break;
					case 18://debuff chance while held: Ichor
						magnitude = (int)Math.Round(scalar * 20);
						info.prefixMagnitude[slot] = magnitude;
						break;

					default:
						break;
				}
			}
			#endregion
			scalar = storedScalar;
			info.prefixIDs[slot] = storedID;
			info.prefixMagnitude[slot] = magnitude;
			info.prefixMagnitudePercent[slot] = scalar;
		}
		public void resetInfo(Item item)
		{
			int prefix = item.prefix;
			item.mana = 0;
			item.crit = 0;
			if (mod.ItemType(item.Name) > 0)
			{
				item.SetDefaults();
			}
			else
			{
				item.SetDefaults(item.type);
			}
			item.Prefix(prefix);
			if (item.type != ItemID.Harpoon)
			{
				item.stack++;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			info.alreadyRolled = false;
			info.prefixIDs = new int[] { -1, -1, -1, -1 };
			info.prefixMagnitude = new int[] { -1, -1, -1, -1 };
			info.prefixMagnitudePercent = new double[] { -1, -1, -1, -1 };
			info.title = "";
			if (Main.netMode == 1)
			{
				var netMessage = mod.GetPacket();
				netMessage.Write("Reset");
				netMessage.Write(item.whoAmI);
				netMessage.Send();
			}
		}
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (item.type == 0)
			{
				return;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			string title = info.title;
			TooltipLine tip = new TooltipLine(mod, "title", title);
			if (info.title.Contains("Transcendent"))
			{
				//transcendent color
				tip.overrideColor = Colors.RarityPurple;
			}
			else if (info.title.Contains("Legendary"))
			{
				//legendary color
				tip.overrideColor = Colors.RarityRed;
			}
			else if (info.title.Contains("Rare"))
			{
				//rare color
				tip.overrideColor = Colors.RarityYellow;
			}
			else if (info.title.Contains("Uncommon"))
			{
				//Uncommon color
				tip.overrideColor = Colors.RarityOrange;
			}
			tooltips.Add(tip);
			int count = 0;
			for (int i = 0; i < 4; i++)
			{
				//Main.NewText("magnitude " + i + " / " + info.prefixMagnitude[i]);
				if (info.prefixMagnitude[i] >= 0)
				{
					count++;
				}
			}
			for (int i = 0; i < count; i++)
			{
				string text = GetPrefixDesc(info.prefixIDs[i], item.damage > 0 && !item.accessory, info.prefixMagnitude[i]);
				string desc = "desc";
				TooltipLine tip2 = new TooltipLine(mod, desc, text);
				float accAdjust = (item.accessory) ? .6f : 1;
				if (info.prefixMagnitudePercent[i] > .9 * accAdjust)
				{
					//transcendent color
					tip2.overrideColor = Colors.RarityPurple;
				}
				else if (info.prefixMagnitudePercent[i] > .725 * accAdjust)
				{
					//legendary color
					tip2.overrideColor = Colors.RarityRed;
				}
				else if (info.prefixMagnitudePercent[i] > .45 * accAdjust)
				{
					//rare color
					tip2.overrideColor = Colors.RarityYellow;
				}
				else if (info.prefixMagnitudePercent[i] > .25 * accAdjust)
				{
					//uncommon color
					tip2.overrideColor = Colors.RarityOrange;
				}
				tooltips.Add(tip2);
				if (info.prefixIDs[i] == 7 && item.mana > 0)
				{
					TooltipLine tip3 = new TooltipLine(mod, "UseMana", "");
					tooltips.Add(tip3);
				}
			}
		}
		public string GetPrefixDesc(int ID, bool weapon, int magnitude)
		{
			string desc = "no data";
			if (weapon)
			{
				switch (ID)
				{
					case 0: desc = "+" + magnitude + "% damage"; break;
					case 1: desc = "+" + magnitude + "% crit chance"; break;
					case 2: desc = "+" + magnitude + "% speed"; break;
					case 3: desc = "+" + magnitude + "% knockback"; break;
					case 4: desc = "+" + magnitude + "% velocity"; break;
					case 5: desc = "-" + magnitude + "% mana cost"; break;
					case 6: desc = magnitude + "% chance not to consume ammo"; break;
					case 7: desc = "+" + magnitude + "% damage, but added mana cost"; break;
					case 8: desc = "+" + magnitude + "% damage during daytime"; break;
					case 9: desc = "+" + magnitude + "% damage during nighttime"; break;
					case 10: desc = "+" + magnitude + "% damage, but cursed"; break;
					case 11: desc = "Up to +" + magnitude * 6 + "% damage based on missing health"; break;
					case 12: desc = "Added damage based on velocity (multiplier: " + magnitude / 2f + "x)"; break;
					case 13: desc = "+" + magnitude + "% chance to inflict Poison"; break;
					case 14: desc = "+" + magnitude + "% chance to inflict On Fire!"; break;
					case 15: desc = "+" + magnitude + "% chance to inflict Frostburn"; break;
					case 16: desc = "+" + magnitude + "% chance to inflict Confusion"; break;
					case 17: desc = "+" + magnitude + "% chance to inflict Cursed Inferno"; break;
					case 18: desc = "+" + magnitude + "% chance to inflict Ichor"; break;
					//case 7: desc = "+" + magnitude + "% tool power"; break;

					default: break;
				}
			}
			else//armor and accessory
			{
				switch (ID)
				{
					case 0: desc = "+" + magnitude + "% melee damage"; break;
					case 1: desc = "+" + magnitude + "% magic damage"; break;
					case 2: desc = "+" + magnitude + "% ranged damage"; break;
					case 3: desc = "+" + magnitude + "% thrown damage"; break;
					case 4: desc = "+" + magnitude + "% minion damage"; break;
					case 5: desc = "+" + magnitude + "% melee crit"; break;
					case 6: desc = "+" + magnitude + "% magic crit"; break;
					case 7: desc = "+" + magnitude + "% ranged crit"; break;
					case 8: desc = "+" + magnitude + "% thrown crit"; break;
					case 9: desc = "Gives off light (+" + magnitude + " strength)"; break;
					case 10: desc = "+" + magnitude + " Max mana"; break;
					case 11: desc = "+" + magnitude + " Max life"; break;
					case 12: desc = "+" + magnitude + "% crit damage"; break;
					case 13: desc = "+" + magnitude + " frames wing time"; break;
					case 14: desc = "+" + magnitude + " luck"; break;
					case 15: desc = "+" + magnitude + "% thorns"; break;
					case 16: desc = "+" + magnitude + "% mining speed"; break;//multiplicative stacking
					case 17: desc = "+" + magnitude + " invincibility frames when hit"; break;
					case 18: desc = "+" + magnitude + "% dodge chance"; break;
					case 19: desc = "Knockback immunity"; break;
					case 20: desc = "+" + magnitude + " fishing skill"; break;
					case 21: desc = "+" + magnitude + "% damage to enemies with full health"; break;//
					case 22: desc = "+" + magnitude + "% chance to survive lethal damage"; break;//
					case 23: desc = "+" + magnitude + "% damage redirected to mana"; break;//
					case 24: desc = "+" + magnitude + "% defense"; break;//
					case 25: desc = "+" + magnitude + " life regen / minute"; break;//
					case 26: desc = "+" + magnitude + "% movespeed and max movespeed"; break;//
					default: break;
				}
			}
			return desc;
		}
		public override bool ConsumeAmmo(Item item, Player player)
		{
			if (item.type == 0)
			{
				return false;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			int chance = 0;
			for (int i = 0; i < 4; i++)
			{
				if (info.prefixIDs[i] == 6)
				{
					chance = info.prefixMagnitude[i];
				}
			}
			if (Main.rand.Next(0, 100) < chance)
			{
				return false;
			}
			return base.ConsumeAmmo(item, player);
		}
		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			if (item.type == 0)
			{
				return;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			int magnitude = 0;
			for (int i = 0; i < 4; i++)
			{
				if (info.prefixIDs[i] == 8 && Main.dayTime)
				{
					magnitude = info.prefixMagnitude[i];
					damage = (int)(damage * (1 + magnitude / 100.0));
				}
				else if (info.prefixIDs[i] == 9 && !Main.dayTime)
				{
					magnitude = info.prefixMagnitude[i];
					damage = (int)(damage * (1 + magnitude / 100.0));
				}
				else if (info.prefixIDs[i] == 11 && player.statLife != 0)
				{
					magnitude = (int)(info.prefixMagnitude[i] * ((player.statLifeMax2 - player.statLife) / (double)player.statLifeMax2) * 6);
					damage = (int)(damage * (1 + magnitude / 100.0));
				}
				else if (info.prefixIDs[i] == 12 && player.velocity.Length() > 0)
				{
					magnitude = (int)(info.prefixMagnitude[i] * player.velocity.Length() / 4);
					damage = (int)(damage * (1 + magnitude / 100.0));
				}
			}
		}
		public override void HoldItem(Item item, Player player)
		{
			if (item.type == 0)
			{
				return;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			MPlayer mplayer = (MPlayer)(player.GetModPlayer(mod, "MPlayer"));
			for (int i = 0; i < 4; i++)
			{
				int magnitude = info.prefixMagnitude[i];
				if (info.prefixIDs[i] == 10)
				{
					mplayer.holdingCursed = true;
				}
				else if (info.prefixIDs[i] == 13)
				{
					mplayer.poisonChance += magnitude;
				}
				else if (info.prefixIDs[i] == 14)
				{
					mplayer.onFireChance += magnitude;
				}
				else if (info.prefixIDs[i] == 15)
				{
					mplayer.frostburnChance += magnitude;
				}
				else if (info.prefixIDs[i] == 16)
				{
					mplayer.confusionChance += magnitude;
				}
				else if (info.prefixIDs[i] == 17)
				{
					mplayer.infernoChance += magnitude;
				}
				else if (info.prefixIDs[i] == 18)
				{
					mplayer.ichorChance += magnitude;
				}
			}
		}
		public override void UpdateEquip(Item item, Player player)
		{
			if (item.type == 0)
			{
				return;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			MPlayer mplayer = (MPlayer)(player.GetModPlayer(mod, "MPlayer"));
			int magnitude = 0;
			for (int i = 0; i < 4; i++)
			{
				magnitude = info.prefixMagnitude[i];
				if (magnitude <= 0 && info.prefixIDs[i] >= 0)
				{
					info.prefixMagnitude[i] = 1;
				}
				if (info.prefixIDs[i] == 0)
				{
					player.meleeDamage += magnitude / 100f;
				}
				else if (info.prefixIDs[i] == 1)
				{
					player.magicDamage += magnitude / 100f;
				}
				else if (info.prefixIDs[i] == 2)
				{
					player.rangedDamage += magnitude / 100f;
				}
				else if (info.prefixIDs[i] == 3)
				{
					player.thrownDamage += magnitude / 100f;
				}
				else if (info.prefixIDs[i] == 4)
				{
					player.minionDamage += magnitude / 100f;
				}
				else if (info.prefixIDs[i] == 5)
				{
					player.meleeCrit += magnitude;
				}
				else if (info.prefixIDs[i] == 6)
				{
					player.magicCrit += magnitude;
				}
				else if (info.prefixIDs[i] == 7)
				{
					player.rangedCrit += magnitude;
				}
				else if (info.prefixIDs[i] == 8)
				{
					player.thrownCrit += magnitude;
				}
				else if (info.prefixIDs[i] == 9)
				{
					mplayer.lightStrength += magnitude;
				}
				else if (info.prefixIDs[i] == 10)
				{
					player.statManaMax2 += magnitude;
				}
				else if (info.prefixIDs[i] == 11)
				{
					player.statLifeMax2 += magnitude;
				}
				else if (info.prefixIDs[i] == 12)
				{
					mplayer.critDamageBoost += magnitude;
				}
				else if (info.prefixIDs[i] == 13)
				{
					player.wingTimeMax += magnitude;
				}
				else if (info.prefixIDs[i] == 14)
				{
					mplayer.luck += magnitude;
				}
				else if (info.prefixIDs[i] == 15)
				{
					player.thorns += magnitude / 100f;
				}
				else if (info.prefixIDs[i] == 16)
				{
					player.pickSpeed *= (1 - magnitude / 100f);
				}
				else if (info.prefixIDs[i] == 17)
				{
					mplayer.bonusImmunity += magnitude;
				}
				else if (info.prefixIDs[i] == 18)
				{
					mplayer.dodgeChance += magnitude;
				}
				else if (info.prefixIDs[i] == 19)
				{
					player.noKnockback = true;
				}
				else if (info.prefixIDs[i] == 20)
				{
					player.fishingSkill += magnitude;
				}
				else if (info.prefixIDs[i] == 21)
				{
					mplayer.bonusDamageToMaxLife += magnitude;
				}
				else if (info.prefixIDs[i] == 22)
				{
					mplayer.miracleChance += magnitude;
				}
				else if (info.prefixIDs[i] == 23)
				{
					mplayer.percentDamageToMana += magnitude;
				}
				else if (info.prefixIDs[i] == 24)
				{
					mplayer.percentDefBoost += magnitude;
				}
				else if (info.prefixIDs[i] == 25)
				{
					mplayer.subLifeRegen += magnitude;
				}
				else if (info.prefixIDs[i] == 26)
				{
					player.moveSpeed += magnitude / 100f;
					player.maxRunSpeed *= (1 + magnitude / 100f);
				}
			}
		}
		public override void UpdateInventory(Item item, Player player)
		{
			if (item.type == 0)
			{
				return;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			for (int i = 0; i < 4; i++)
			{
				if (info.prefixMagnitude[i] <= 0 && info.prefixIDs[i] >= 0)
				{
					info.prefixMagnitude[i] = 1;
				}
			}
			if (info.prefixMagnitude[0] > 0)
			{
				//Main.NewText("Magnitude at update: " + info.prefixMagnitude[0]);
			}
		}
		public override bool CanRightClick(Item item)
		{
			if (item.type == 0)
			{
				return base.CanRightClick(item);
			}
			if (Main.player[item.owner].inventory[Main.player[item.owner].selectedItem].type == mod.ItemType("MagicDice"))
			{
				EMMItem info = item.GetGlobalItem<EMMItem>();
				if (!info.alreadyRolled)
				{
					return true;
				}
			}
			return base.CanRightClick(item);
		}
		public override void RightClick(Item item, Player player)
		{
			Item dice = Main.player[item.owner].inventory[Main.player[item.owner].selectedItem];
			if (dice.type == mod.ItemType("MagicDice"))
			{
				item.stack++;
				Main.player[item.owner].inventory[Main.player[item.owner].selectedItem].stack--;
				if (Main.myPlayer == player.whoAmI && Main.mouseItem != null)
				{
					Main.mouseItem.stack--;
				}
				if (Main.player[item.owner].inventory[Main.player[item.owner].selectedItem].stack <= 0)
				{
					Main.player[item.owner].inventory[Main.player[item.owner].selectedItem] = new Item();
				}
				resetInfo(item);
				rollWithLuck(item, player);
			}
		}
		public override bool NeedsSaving(Item item)
		{
			if (item.type == 0 || item.consumable || item.ammo > 0 || item.type == ModLoader.GetMod("ModLoader").ItemType("MysteryItem"))
			{
				return false;
			}
			return true;
		}
		public override TagCompound Save(Item item)
		{
			if (item.type == 0 || item.type == ModLoader.GetMod("ModLoader").ItemType("MysteryItem"))
			{
				return null;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			TagCompound tag = new TagCompound();
			tag.Add("IDs", info.prefixIDs);
			tag.Add("Magnitude0", info.prefixMagnitudePercent[0]);
			tag.Add("Magnitude1", info.prefixMagnitudePercent[1]);
			tag.Add("Magnitude2", info.prefixMagnitudePercent[2]);
			tag.Add("Magnitude3", info.prefixMagnitudePercent[3]);
			tag.Add("Rolled?", info.alreadyRolled);
			tag.Add("Title", info.title);

			return tag;
		}
		public override void Load(Item item, TagCompound tag)
		{
			if (item.type == 0 || tag == null || item.type == ModLoader.GetMod("ModLoader").ItemType("MysteryItem"))
			{
				return;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			int[] ids = tag.GetIntArray("IDs");
			double[] magnitudes = new double[4];
			magnitudes[0] = tag.GetAsDouble("Magnitude0");
			magnitudes[1] = tag.GetAsDouble("Magnitude1");
			magnitudes[2] = tag.GetAsDouble("Magnitude2");
			magnitudes[3] = tag.GetAsDouble("Magnitude3");
			info.alreadyRolled = tag.GetBool("Rolled?");
			info.title = tag.GetString("Title");
			for (int i = 0; i < ids.Length; i++)
			{
				try
				{
					addPrefix(item, ids[i], (float)magnitudes[i] / ((item.accessory) ? .6f : 1), i);
				}
				catch
				{
					ErrorLogger.Log("Save Out of Bounds Error! Please report this.");
				}
			}
		}
		public override void LoadLegacy(Item item, BinaryReader reader)
		{
			if (item.type == 0 || reader.PeekChar() == -1 || item.type == ModLoader.GetMod("ModLoader").ItemType("MysteryItem"))
			{
				return;
			}
			EMMItem info = item.GetGlobalItem<EMMItem>();
			for (int i = 0; i < 4; i++)
			{
				try
				{
					int id = reader.ReadInt32();
					float scalar = (float)(reader.ReadDouble()) / ((item.accessory) ? .6f : 1);
					addPrefix(item, id, scalar, i);
				}
				catch
				{

				}
			}
			info.alreadyRolled = reader.ReadBoolean();
			info.title = reader.ReadString();
		}
		/*Obsolete code
        public override void SaveCustomData(Item item, BinaryWriter writer)
        {
            if (item.type == 0 || item.type == ModLoader.GetMod("ModLoader").ItemType("MysteryItem"))
            {
                return;
            }
            ItemInf info = (ItemInf)item.GetModInfo(mod, "ItemInf");
            for(int i = 0; i < 4; i++)
            {
                writer.Write(info.prefixIDs[i]);
                writer.Write(prefixMagnitudePercent[i]);
            }
            writer.Write(alreadyRolled);
            writer.Write(title);
        }*/
	}
}
