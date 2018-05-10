using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot
{
	//[ComVisible(true)]
	//[Flags]
	//public enum CustomReforgeMode : byte
	//{
	//	Vanilla = 1,
	//	ForceWeapon = 2,
	//	ForceAccessory = 4,
	//	ForceSimulate = 8,
	//	Custom = 16
	//}

	/// <summary>
	/// Defines an item that may be modified by modifiers from mods
	/// </summary>
	public sealed class EMMItem : GlobalItem
	{
		// Helpers
		public static EMMItem GetItemInfo(Item item) => item.GetGlobalItem<EMMItem>();
		public static IEnumerable<Modifier> GetActivePool(Item item) => GetItemInfo(item)?.ModifierPool?.ActiveModifiers ?? Enumerable.Empty<Modifier>();

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public ModifierPool ModifierPool;
		public bool HasRolled;
		//public CustomReforgeMode CustomReforgeMode = CustomReforgeMode.ForceWeapon;

		/// <summary>
		/// Attempts to roll new modifiers
		/// Has a set chance to hit a predefined pool of modifiers
		/// </summary>
		internal ModifierPool RollNewPool(ModifierContext ctx)
		{
			// Now we have actually rolled
			HasRolled = true;

			// Try getting a weighted pool, or roll all modifiers at random
			bool rollPredefinedPool = Main.rand.NextFloat() <= 0.25f;
			bool canRollRandom = !rollPredefinedPool;

			if (rollPredefinedPool)
			{
				ModifierPool = EMMLoader.GetWeightedPool(ctx);
				canRollRandom = ModifierPool == null;
			}

			if (canRollRandom)
			{
				ModifierPool = Loot.Instance.GetModifierPool<AllModifiersPool>();
				if (!ModifierPool._CanRoll(ctx))
				{
					ModifierPool = null;
					return null;
				}
			}

			// Attempt rolling modifiers
			if (!ModifierPool.RollModifiers(ctx))
			{
				ModifierPool = null; // reset (didn't roll anything)
			}
			else
			{
				ModifierPool.Modifiers = null;
				ModifierPool.UpdateRarity();
			}

			return ModifierPool;
		}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			EMMItem clone = (EMMItem)base.Clone(item, itemClone);
			clone.ModifierPool = (ModifierPool)ModifierPool?.Clone();
			// there is no need to apply here, we already cloned the item which stats are already modified by its pool
			return clone;
		}

		public override void Load(Item item, TagCompound tag)
		{
			// enforce illegitimate rolls to go away
			if (!ModifierPool.IsValidFor(item))
			{
				ModifierPool = null;
			}
			else if (tag.ContainsKey("Type"))
			{
				ModifierPool = ModifierPool._Load(item, tag);

				// enforce illegitimate rolls to go away
				if (ModifierPool.ActiveModifiers == null || ModifierPool.ActiveModifiers.Length <= 0)
					ModifierPool = null;
			}

			HasRolled = tag.GetBool("HasRolled");

			ModifierPool?.ApplyModifiers(item);
		}

		public override TagCompound Save(Item item)
		{
			TagCompound tag = ModifierPool != null
				? ModifierPool.Save(item, ModifierPool)
				: new TagCompound();

			tag.Add("HasRolled", HasRolled);

			return tag;
		}

		public override bool NeedsSaving(Item item)
			=> ModifierPool != null || HasRolled;

		public override void NetReceive(Item item, BinaryReader reader)
		{
			if (reader.ReadBoolean())
				ModifierPool = ModifierPool._NetReceive(item, reader);

			HasRolled = reader.ReadBoolean();

			ModifierPool?.ApplyModifiers(item);
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			bool hasPool = ModifierPool != null;
			writer.Write(hasPool);
			if (hasPool)
				ModifierPool._NetSend(ModifierPool, item, writer);

			writer.Write(HasRolled);
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnCraft,
				Item = item,
				Player = Main.LocalPlayer,
				Recipe = recipe
			};

			ModifierPool pool = GetItemInfo(item).ModifierPool;
			if (!HasRolled && pool == null)
			{
				pool = RollNewPool(ctx);
				pool?.ApplyModifiers(item);
			}

			base.OnCraft(item, recipe);
		}

		public override bool OnPickup(Item item, Player player)
		{
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnPickup,
				Item = item,
				Player = player
			};

			ModifierPool pool = GetItemInfo(item).ModifierPool;
			if (!HasRolled && pool == null)
			{
				pool = RollNewPool(ctx);
				pool?.ApplyModifiers(item);
			}

			return base.OnPickup(item, player);
		}

		public override void PostReforge(Item item)
		{
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnReforge,
				Item = item,
				Player = Main.LocalPlayer
			};

			ModifierPool pool = RollNewPool(ctx);
			pool?.ApplyModifiers(item);
		}

		private string GetPrefixNormString(float cpStat, float rStat, ref double num, ref Color? color)
		{
			//float num19 = (float)Main.mouseTextColor / 255f;
			//patch file: num20
			float num20 = (float)Main.mouseTextColor / 255f;
			int a = (int)Main.mouseTextColor;

			if (cpStat == 0f && rStat != 0f)
			{
				num = 1;
				if (rStat > 0f)
				{
					color = new Color((int)((byte)(120f * num20)), (int)((byte)(190f * num20)), (int)((byte)(120f * num20)), a);
					return "+" + rStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
				}
				else
				{
					color = new Color((int)((byte)(190f * num20)), (int)((byte)(120f * num20)), (int)((byte)(120f * num20)), a);
					return rStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
				}
			}

			double num12 = (double)((float)rStat - (float)cpStat);
			num12 = num12 / (double)((float)cpStat) * 100.0;
			num12 = Math.Round(num12);
			num = num12;

			if (num12 > 0.0)
			{
				color = new Color((int)((byte)(120f * num20)), (int)((byte)(190f * num20)), (int)((byte)(120f * num20)), a);
				return "+" + num12.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
			}
			else
			{
				color = new Color((int)((byte)(190f * num20)), (int)((byte)(120f * num20)), (int)((byte)(120f * num20)), a);
				return num12.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
			}
			//if (num12 < 0.0)
			//{
			//	array3[num4] = true;
			//}
		}

		/// <summary>
		/// Will modify vanilla tooltips to add additional information for the affected item's modifiers
		/// </summary>
		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			var pool = GetItemInfo(item).ModifierPool;
			if (pool != null && pool.ActiveModifiers.Length > 0)
			{
				var vanillaTooltips = tooltips.Where(x => x.mod.Equals("Terraria")).ToArray();
				var baseItem = new Item();
				baseItem.netDefaults(item.netID);

				var poolItem = baseItem.CloneWithModdedDataFrom(item);
				GetItemInfo(poolItem)?.ModifierPool.ApplyModifiers(poolItem);
				//var info = GetItemInfo(poolItem);
				//if (!info.CustomReforgeMode.HasFlag(CustomReforgeMode.Vanilla))
				//{
				//	ItemHack.ModifyItemCustomReforce(GetItemInfo(baseItem), baseItem);
				//	ItemHack.ModifyItemCustomReforce(info, poolItem);
				//	if (poolItem.modItem != null && info.CustomReforgeMode.HasFlag(CustomReforgeMode.Custom))
				//	{
				//		var EMMCustomReforge = poolItem.modItem.GetType().GetMethod("EMMCustomReforge");
				//		EMMCustomReforge?.Invoke(poolItem.modItem, null);
				//	}
				//}

				bool chkDamage = poolItem.damage != item.damage;
				bool chkSpeed = poolItem.useAnimation != baseItem.useAnimation;
				bool chkCritChance = poolItem.crit != baseItem.crit;
				bool chkUseMana = poolItem.mana != baseItem.mana;
				bool chkSize = poolItem.scale != baseItem.scale;
				bool chkShootSpeed = poolItem.shootSpeed != baseItem.shootSpeed;
				bool chkKnockback = poolItem.knockBack != baseItem.knockBack;

				try
				{
					foreach (var vttl in vanillaTooltips)
					{
						//int number = Int32.Parse(new string(vttl.text.Where(char.IsNumber).ToArray()));
						double outNumber = 0d;
						string newTT = vttl.text;
						Color? newC = vttl.overrideColor;
						string TTend = new string(vttl.text.Reverse().ToArray().TakeWhile(x => !char.IsDigit(x)).Reverse().ToArray());

						//private string[] _prefixTooltipLines = {
						//		"PrefixDamage", "PrefixSpeed", "PrefixCritChance", "PrefixUseMana", "PrefixSize",
						//		"PrefixShootSpeed", "PrefixKnockback", "PrefixAccDefense", "PrefixAccMaxMana",
						//		"PrefixAccCritChance", "PrefixAccDamage", "PrefixAccMoveSpeed", "PrefixAccMeleeSpeed"
						//	};

						if (vttl.Name.Equals("PrefixDamage") && chkDamage)
						{
							newTT = GetPrefixNormString(poolItem.damage, item.damage, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixSpeed") && chkSpeed)
						{
							newTT = GetPrefixNormString(item.useAnimation, poolItem.useAnimation, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixCritChance") && chkCritChance)
						{
							newTT = GetPrefixNormString(poolItem.crit, item.crit, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixUseMana") && chkUseMana)
						{
							if (baseItem.mana != 0)
								newTT = GetPrefixNormString(poolItem.mana, item.mana, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixSize") && chkSize)
						{
							newTT = GetPrefixNormString(poolItem.scale, item.scale, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixShootSpeed") && chkShootSpeed)
						{
							newTT = GetPrefixNormString(poolItem.shootSpeed, item.shootSpeed, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixKnockback") && chkKnockback)
						{
							newTT = GetPrefixNormString(poolItem.knockBack, item.knockBack, ref outNumber, ref newC);
						}
						else
						{
							continue;
						}

						int ttlI = tooltips.FindIndex(x => x.mod.Equals(vttl.mod) && x.Name.Equals(vttl.Name));
						if (ttlI != -1)
						{
							if (outNumber == 0d)
							{
								tooltips.RemoveAt(ttlI);
							}
							else
							{
								tooltips[ttlI].text = $"{newTT}{TTend}";
								tooltips[ttlI].overrideColor = newC;
							}
						}
					}
				}
				catch (Exception e)
				{
					Main.NewTextMultiline(e.ToString());
				}

				var useManaTT = vanillaTooltips.FirstOrDefault(x => x.mod.Equals("Terraria") && x.Name.Equals("UseMana"));
				if (useManaTT != null)
				{
					if (poolItem.mana > baseItem.mana)
					{
						string foundMana = new string(useManaTT.text.Where(char.IsDigit).ToArray());
						if (foundMana != string.Empty)
							useManaTT.text = useManaTT.text.Replace(foundMana, poolItem.mana.ToString());
					}
				}

				int i = tooltips.FindIndex(x => x.mod == "Terraria" && x.Name == "ItemName");
				if (i != -1)
				{
					var namelayer = tooltips[i];
					if (pool.Rarity.ItemPrefix != null)
						namelayer.text = $"{pool.Rarity.ItemPrefix} {namelayer.text}";
					if (pool.Rarity.ItemSuffix != null)
						namelayer.text += $" {pool.Rarity.ItemSuffix}";
					if (pool.Rarity.OverrideNameColor != null)
						namelayer.overrideColor = pool.Rarity.OverrideNameColor;
					tooltips[i] = namelayer;
				}

				i = tooltips.Count;
				tooltips.Insert(i, new TooltipLine(mod, "Modifier:Name", $"[{pool.Rarity.Name}]") { overrideColor = pool.Rarity.Color * Main.inventoryScale });

				foreach (var ttcol in pool.Description)
					foreach (var tt in ttcol)
						tooltips.Insert(++i, new TooltipLine(mod, $"Modifier:Description:{i}", tt.Text) { overrideColor = (tt.Color ?? Color.White) * Main.inventoryScale });

				foreach (var e in pool.ActiveModifiers)
					e.ModifyTooltips(item, tooltips);
			}
		}
	}

}
