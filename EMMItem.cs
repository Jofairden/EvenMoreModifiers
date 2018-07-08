using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Loot.Core;
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

		public ModifierPool ModifierPool; // the current pool of mods. null if none.
		public bool HasRolled; // has rolled a pool
		public bool SealedModifiers; // are modifiers unchangeable

		// Non saved
		public bool JustTinkerModified; // is just tinker modified: e.g. armor hacked
		public bool SlottedInCubeUI; // is currently in cube UI slot


		public const int SaveVersion = 2;
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
			EMMItem clone = (EMMItem) base.Clone(item, itemClone);
			clone.ModifierPool = (ModifierPool) ModifierPool?.Clone();
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

			// SaveVersion >= 2
			if (tag.ContainsKey("SaveVersion"))
			{
				HasRolled = tag.GetBool("HasRolled");
				SealedModifiers = tag.GetBool("SealedModifiers");
			}
			else // SaveVersion 1
			{
				HasRolled = tag.GetBool("HasRolled");
			}

			ModifierPool?.ApplyModifiers(item);
		}

		public override TagCompound Save(Item item)
		{
			TagCompound tag = ModifierPool != null
				? ModifierPool.Save(item, ModifierPool)
				: new TagCompound();

			tag.Add("HasRolled", HasRolled);

			// SaveVersion saved since SaveVersion 2, version 1 not present
			tag.Add("SaveVersion", SaveVersion);
			tag.Add("SealedModifiers", SealedModifiers);

			return tag;
		}

		public override bool NeedsSaving(Item item)
			=> ModifierPool != null || HasRolled;

		public override void NetReceive(Item item, BinaryReader reader)
		{
			if (reader.ReadBoolean())
				ModifierPool = ModifierPool._NetReceive(item, reader);

			HasRolled = reader.ReadBoolean();
			SealedModifiers = reader.ReadBoolean(); // Since SaveVersion 2

			ModifierPool?.ApplyModifiers(item);

			JustTinkerModified = reader.ReadBoolean();
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			bool hasPool = ModifierPool != null;
			writer.Write(hasPool);
			if (hasPool)
				ModifierPool._NetSend(ModifierPool, item, writer);

			writer.Write(HasRolled);
			writer.Write(SealedModifiers); // Since SaveVersion 2

			writer.Write(JustTinkerModified);
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			ModifierPool pool = GetItemInfo(item).ModifierPool;
			if (!HasRolled && pool == null)
			{
				ModifierContext ctx = new ModifierContext
				{
					Method = ModifierContextMethod.OnCraft,
					Item = item,
					Player = Main.LocalPlayer,
					Recipe = recipe
				};

				pool = RollNewPool(ctx);
				pool?.ApplyModifiers(item);
			}

			base.OnCraft(item, recipe);
		}

		public override bool OnPickup(Item item, Player player)
		{
			ModifierPool pool = GetItemInfo(item).ModifierPool;
			if (!HasRolled && pool == null)
			{
				ModifierContext ctx = new ModifierContext
				{
					Method = ModifierContextMethod.OnPickup,
					Item = item,
					Player = player
				};

				pool = RollNewPool(ctx);
				pool?.ApplyModifiers(item);
			}

			return base.OnPickup(item, player);
		}

		public override void PostReforge(Item item)
		{
			if (!SealedModifiers)
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
		}

		private string GetPrefixNormString(float cpStat, float rStat, ref double num, ref Color? color)
		{
			//float num19 = (float)Main.mouseTextColor / 255f;
			//patch file: num20
			float defColorVal = Main.mouseTextColor / 255f;
			int alphaColor = Main.mouseTextColor;

			if (cpStat == 0f && rStat != 0f)
			{
				num = 1;
				if (rStat > 0f)
				{
					color = new Color((byte) (120f * defColorVal), (byte) (190f * defColorVal), (byte) (120f * defColorVal), alphaColor);
					return "+" + rStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
				}

				color = new Color((byte) (190f * defColorVal), (byte) (120f * defColorVal), (byte) (120f * defColorVal), alphaColor);
				return rStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
			}

			double diffStat = rStat - cpStat;
			diffStat = diffStat / cpStat * 100.0;
			diffStat = Math.Round(diffStat);
			num = diffStat;

			// for some reason - is handled automatically, but + is not
			if (diffStat > 0.0)
			{
				color = new Color((byte) (120f * defColorVal), (byte) (190f * defColorVal), (byte) (120f * defColorVal), alphaColor);
				return "+" + diffStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
			}

			color = new Color((byte) (190f * defColorVal), (byte) (120f * defColorVal), (byte) (120f * defColorVal), alphaColor);
			return diffStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
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
				// the following part, recalculate the vanilla prefix tooltips
				// this is because our mods modify the stats, which was never intended by vanilla, causing the differences to be innacurate and bugged

				// RECALC START
				var vanillaTooltips = tooltips.Where(x => x.mod.Equals("Terraria")).ToArray();
				var baseItem = new Item();
				baseItem.netDefaults(item.netID);

				// the item with just the modifiers applied
//				var poolItem = baseItem.CloneWithModdedDataFrom(item);
//				GetItemInfo(poolItem)?.ModifierPool.ApplyModifiers(poolItem);

				// the item with just the prefix applied
				var prefixItem = baseItem.Clone();
				prefixItem.Prefix(item.prefix);


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

						if (vttl.Name.Equals("PrefixDamage"))
						{
							if (baseItem.damage > 0)
								newTT = GetPrefixNormString(baseItem.damage, prefixItem.damage, ref outNumber, ref newC);
							else
								newTT = GetPrefixNormString(prefixItem.damage, baseItem.damage, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixSpeed"))
						{
							if (baseItem.useAnimation <= 0)
								newTT = GetPrefixNormString(baseItem.useAnimation, prefixItem.useAnimation, ref outNumber, ref newC);
							else
								newTT = GetPrefixNormString(prefixItem.useAnimation, baseItem.useAnimation, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixCritChance"))
						{
							outNumber = prefixItem.crit - baseItem.crit;
							float defColorVal = Main.mouseTextColor / 255f;
							int alphaColor = Main.mouseTextColor;
							newTT = "";
							if (outNumber >= 0)
							{
								newTT += "+";
								newC = new Color((byte) (120f * defColorVal), (byte) (190f * defColorVal), (byte) (120f * defColorVal), alphaColor);
							}
							else
							{
								newC = new Color((byte) (190f * defColorVal), (byte) (120f * defColorVal), (byte) (120f * defColorVal), alphaColor);
							}

							newTT += outNumber.ToString(CultureInfo.InvariantCulture);
						}
						else if (vttl.Name.Equals("PrefixUseMana"))
						{
							if (baseItem.mana != 0)
							{
								float defColorVal = Main.mouseTextColor / 255f;
								int alphaColor = Main.mouseTextColor;
								newTT = GetPrefixNormString(baseItem.mana, prefixItem.mana, ref outNumber, ref newC);
								if (prefixItem.mana < baseItem.mana)
									newC = new Color((byte) (120f * defColorVal), (byte) (190f * defColorVal), (byte) (120f * defColorVal), alphaColor);
								else
									newC = new Color((byte) (190f * defColorVal), (byte) (120f * defColorVal), (byte) (120f * defColorVal), alphaColor);
							}
						}
						else if (vttl.Name.Equals("PrefixSize"))
						{
							if (baseItem.scale > 0)
								newTT = GetPrefixNormString(baseItem.scale, prefixItem.scale, ref outNumber, ref newC);
							else
								newTT = GetPrefixNormString(prefixItem.scale, baseItem.scale, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixShootSpeed"))
						{
							if (baseItem.shootSpeed > 0)
								newTT = GetPrefixNormString(baseItem.shootSpeed, prefixItem.shootSpeed, ref outNumber, ref newC);
							else
								newTT = GetPrefixNormString(prefixItem.shootSpeed, baseItem.shootSpeed, ref outNumber, ref newC);
						}
						else if (vttl.Name.Equals("PrefixKnockback"))
						{
							if (baseItem.knockBack > 0)
								newTT = GetPrefixNormString(baseItem.knockBack, prefixItem.knockBack, ref outNumber, ref newC);
							else
								newTT = GetPrefixNormString(prefixItem.knockBack, baseItem.knockBack, ref outNumber, ref newC);
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
					// Hopefully never happens
					Main.NewTextMultiline(e.ToString());
				}
				// RECALC END

				// Modifies the "Uses X Mana" line to match our mods
//				var useManaTT = vanillaTooltips.FirstOrDefault(x => x.mod.Equals("Terraria") && x.Name.Equals("UseMana"));
//				if (useManaTT != null)
//				{
//					if (poolItem.mana > baseItem.mana)
//					{
//						string foundMana = new string(useManaTT.text.Where(char.IsDigit).ToArray());
//						if (foundMana != string.Empty)
//							useManaTT.text = useManaTT.text.Replace(foundMana, poolItem.mana.ToString());
//					}
//				}

				// Modifies the tooltips, to insert generic mods data
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

				// Insert modifier rarity
				i = tooltips.Count;
				tooltips.Insert(i, new TooltipLine(mod, "Loot: Modifier:Rarity", $"[{pool.Rarity.Name}]") {overrideColor = pool.Rarity.Color * Main.inventoryScale});

				// Insert lines
				foreach (var ttcol in pool.Description)
				foreach (var tt in ttcol)
				{
					tooltips.Insert(++i, new TooltipLine(mod, $"Loot: Modifier:Line:{i}", tt.Text) {overrideColor = (tt.Color ?? Color.White) * Main.inventoryScale});
				}

				// Insert sealed notation
				if (SealedModifiers)
				{
					var ttl = new TooltipLine(mod, "Loot: Modifier:Sealed", "Modifiers cannot be changed")
					{
						overrideColor = Color.Cyan
					};
					tooltips.Insert(++i, ttl);
				}

				// Call modify tooltips
				foreach (var e in pool.ActiveModifiers) e.ModifyTooltips(item, tooltips);
			}
		}
	}
}