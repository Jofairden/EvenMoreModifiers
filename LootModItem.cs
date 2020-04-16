using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Hacks;
using Loot.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot
{
	/// <summary>
	/// Defines an item that may be modified by modifiers from mods
	/// </summary>
	public sealed class LootModItem : GlobalItem
	{
		private const int SAVE_VERSION = 14;

		public static LootModItem GetInfo(Item item) => item.GetGlobalItem<LootModItem>();
		public static List<Modifier> GetActivePool(Item item) => GetInfo(item).Modifiers?.Modifiers ?? new List<Modifier>();

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public ModifierRarity Rarity { get; internal set; } // the current rarity
		public FiniteModifierPool Modifiers { get; internal set; } // the current pool of mods.

		public bool HasRolled { get; internal set; } // has rolled a pool
		public bool SealedModifiers { get; internal set; } // are modifiers unchangeable

		// Non saved
		public bool JustTinkerModified { get; internal set; } // is just tinker modified: e.g. armor hacked
		public bool SlottedInUI { get; internal set; } // is currently in cube UI slot

		/// <summary>
		/// Keeps track of if the particular item was activated ('delegated')
		/// Specific usecase see CursedEffect and modifier
		/// </summary>
		public bool IsActivated { get; internal set; }

		private void InvalidateRolls()
		{
			Rarity = mod.GetNullModifierRarity();
			Modifiers = mod.GetNullModifierPool();
		}

		/// <summary>
		/// Attempts to roll new modifiers
		/// Has a set chance to hit a predefined pool of modifiers
		/// </summary>
		//public ModifierPool RollNewPool(ModifierContext ctx, RollingStrategyProperties rollingStrategyProperties = null)
		//{
		//	if (rollingStrategyProperties == null)
		//	{
		//		rollingStrategyProperties = new RollingStrategyProperties();
		//	}

		//	HasRolled = true;
		//	bool noForce = true;

		//	// Custom rarity provided
		//	if (rollingStrategyProperties.OverrideRollModifierRarity != null)
		//	{
		//		Rarity = rollingStrategyProperties.OverrideRollModifierRarity.Invoke();
		//	}
		//	else if (rollingStrategyProperties.ForceModifierRarity != null)
		//	{
		//		Rarity = rollingStrategyProperties.ForceModifierRarity;
		//	}
		//	else if (Rarity == null || Rarity.Type == 0)
		//	{
		//		Rarity = mod.GetModifierRarity<CommonRarity>();
		//	}

		//	ctx.Rarity = Rarity;

		//	// Upgrade rarity
		//	if (rollingStrategyProperties.CanUpgradeRarity(ctx)
		//		&& Main.rand.NextFloat() <= (Rarity.UpgradeChance ?? 0f))
		//	{
		//		var newRarity = Rarity.Upgrade;
		//		var newFromLoader = ModUtils.GetModifierRarity(newRarity);
		//		if (newRarity != null && newFromLoader != null)
		//		{
		//			Rarity = newFromLoader;
		//		}
		//	}
		//	// Downgrade rarity
		//	else if (rollingStrategyProperties.CanDowngradeRarity(ctx)
		//			&& Main.rand.NextFloat() <= (Rarity.DowngradeChance ?? 0f))
		//	{
		//		var newRarity = Rarity.Downgrade;
		//		var newFromLoader = ModUtils.GetModifierRarity(newRarity);
		//		if (newRarity != null && newFromLoader != null)
		//		{
		//			Rarity = newFromLoader;
		//		}
		//	}

		//	ctx.Rarity = Rarity;

		//	// Custom pool provided
		//	if (rollingStrategyProperties.OverrideRollModifierPool != null)
		//	{
		//		ModifierPool = rollingStrategyProperties.OverrideRollModifierPool.Invoke();
		//		noForce = !ModifierPool?._CanRoll(ctx) ?? true;
		//	}

		//	// No behavior provided
		//	if (noForce)
		//	{
		//		// A pool is forced to roll
		//		if (rollingStrategyProperties.ForceModifierPool != null)
		//		{
		//			ModifierPool = ModUtils.GetModifierPool(rollingStrategyProperties.ForceModifierPool.GetType());
		//			noForce = !ModifierPool?._CanRoll(ctx) ?? true;
		//		}

		//		// No pool forced to roll or it's not valid
		//		if (noForce)
		//		{
		//			// Try rolling a predefined (weighted) pool
		//			bool rollPredefinedPool = Main.rand.NextFloat() <= rollingStrategyProperties.RollPredefinedPoolChance;
		//			noForce = !rollPredefinedPool;

		//			if (rollPredefinedPool)
		//			{
		//				// GetWeightedPool already checks _CanRoll
		//				ModifierPool = ContentLoader.ModifierPool.GetWeightedPool(ctx);
		//				noForce = ModifierPool == null || !ModifierPool._CanRoll(ctx);
		//			}

		//			// Roll from all modifiers
		//			if (noForce)
		//			{
		//				ModifierPool = mod.GetAllModifiersPool();
		//				if (!ModifierPool._CanRoll(ctx))
		//				{
		//					InvalidateRolls();
		//					return null;
		//				}
		//			}
		//		}
		//	}

		//	if (ctx.Strategy != null)
		//	{
		//		// Attempt rolling modifiers
		//		if (!ctx.Strategy.Roll(ctx, new RollingStrategyContext(ctx.Item, rollingStrategyProperties)))
		//		{
		//			InvalidateRolls();
		//		}
		//	}

		//	ctx.Item.GetGlobalItem<GraphicsGlobalItem>().NeedsUpdate = true;
		//	return ModifierPool;
		//}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			LootModItem clone = (LootModItem)base.Clone(item, itemClone);
			clone.Rarity = (ModifierRarity)Rarity?.Clone() ?? mod.GetNullModifierRarity();
			clone.Modifiers = (FiniteModifierPool)Modifiers?.Clone() ?? mod.GetNullModifierPool();
			// there is no need to apply here, we already cloned the item which stats are already modified by its pool
			return clone;
		}

		public override void Load(Item item, TagCompound tag)
		{
			// enforce illegitimate rolls to go away (needed for earliest versions saves)
			if (!item.IsModifierRollableItem())
			{
				InvalidateRolls();
			}
			else if (tag.ContainsKey("SaveVersion"))
			{
				int saveVersion = tag.GetInt("SaveVersion");
				if (saveVersion < 14)
				{
					InvalidateRolls();
				}
				else
				{
					Rarity = ModifierRarityIO.Load(item, tag.GetCompound("ModifierRarity"));
					Modifiers = ModifierPoolIO.Load(item, tag.GetCompound("ModifierPool"));
					SealedModifiers = tag.GetBool("SealedModifiers");
					HasRolled = tag.GetBool("HasRolled");
					Modifiers.Apply(item);
				}
			}
		}

		public override TagCompound Save(Item item)
		{
			TagCompound tag = new TagCompound
			{
				// SaveVersion saved since SaveVersion 2, version 1 not present
				{"SaveVersion", SAVE_VERSION},
				{"SealedModifiers", SealedModifiers},
				{"HasRolled", HasRolled},
				{"ModifierRarity", ModifierRarityIO.Save(item, Rarity)},
				{"ModifierPool", ModifierPoolIO.Save(item, Modifiers)}
			};

			return tag;
		}

		public override bool NeedsSaving(Item item)
			=> Modifiers != null || HasRolled;

		public override void NetReceive(Item item, BinaryReader reader)
		{
			bool hasPool = reader.ReadBoolean();

			if (hasPool)
			{
				Rarity = ModifierRarityIO.NetReceive(item, reader);
				Modifiers = ModifierPoolIO.NetReceive(item, reader);
			}

			HasRolled = reader.ReadBoolean();
			SealedModifiers = reader.ReadBoolean();
			JustTinkerModified = reader.ReadBoolean();

			item.UpdateModifiers(Modifiers.Modifiers);
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			bool hasPool = Modifiers != null;
			writer.Write(hasPool);

			if (hasPool)
			{
				ModifierRarityIO.NetSend(Rarity, item, writer);
				ModifierPoolIO.NetSend(Modifiers, item, writer);
			}

			writer.Write(HasRolled);
			writer.Write(SealedModifiers);

			writer.Write(JustTinkerModified);
		}

		//public override void OnCraft(Item item, Recipe recipe)
		//{
		//	ModifierPool pool = GetInfo(item).ModifierPool;
		//	if (!HasRolled && pool == null)
		//	{
		//		ModifierContext ctx = new ModifierContext
		//		{
		//			Method = ModifierContextMethod.OnCraft,
		//			Item = item,
		//			Player = Main.LocalPlayer,
		//			Recipe = recipe
		//		};

		//		pool = RollNewPool(ctx);
		//		pool?.ApplyModifiers(item);
		//	}

		//	base.OnCraft(item, recipe);
		//}

		//public override bool OnPickup(Item item, Player player)
		//{
		//	ModifierPool pool = GetInfo(item).ModifierPool;
		//	if (!HasRolled && pool == null)
		//	{
		//		ModifierContext ctx = new ModifierContext
		//		{
		//			Method = ModifierContextMethod.OnPickup,
		//			Item = item,
		//			Player = player,
		//			Strategy = RollingUtils.Strategies.Normal
		//		};

		//		pool = RollNewPool(ctx);
		//		pool?.ApplyModifiers(item);
		//	}

		//	return base.OnPickup(item, player);
		//}

		//public override void PostReforge(Item item)
		//{
		//	if (!SealedModifiers)
		//	{
		//		ModifierContext ctx = new ModifierContext
		//		{
		//			Method = ModifierContextMethod.OnReforge,
		//			Item = item,
		//			Player = Main.LocalPlayer,
		//			Strategy = RollingUtils.Strategies.Normal
		//		};

		//		ModifierPool pool = RollNewPool(ctx);
		//		pool?.ApplyModifiers(item);
		//	}
		//}

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
					color = new Color((byte)(120f * defColorVal), (byte)(190f * defColorVal), (byte)(120f * defColorVal), alphaColor);
					return "+" + rStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
				}

				color = new Color((byte)(190f * defColorVal), (byte)(120f * defColorVal), (byte)(120f * defColorVal), alphaColor);
				return rStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
			}

			double diffStat = rStat - cpStat;
			diffStat = diffStat / cpStat * 100.0;
			diffStat = Math.Round(diffStat);
			num = diffStat;

			// for some reason - is handled automatically, but + is not
			if (diffStat > 0.0)
			{
				color = new Color((byte)(120f * defColorVal), (byte)(190f * defColorVal), (byte)(120f * defColorVal), alphaColor);
				return "+" + diffStat.ToString(CultureInfo.InvariantCulture); /* + Lang.tip[39].Value;*/
			}

			color = new Color((byte)(190f * defColorVal), (byte)(120f * defColorVal), (byte)(120f * defColorVal), alphaColor);
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
			var pool = Modifiers;
			if (pool != null)
			{
				// the following part, recalculate the vanilla prefix tooltips
				// this is because our mods modify the stats, which was never intended by vanilla, causing the differences to be innacurate and bugged

				// RECALC START
				var vanillaTooltips = tooltips.Where(x => x.mod.Equals("Terraria")).ToArray();
				var baseItem = new Item();
				baseItem.netDefaults(item.netID);

				// the item with just the modifiers applied
				// var poolItem = baseItem.CloneWithModdedDataFrom(item);
				// GetItemInfo(poolItem)?.ModifierPool.ApplyModifiers(poolItem);

				// the item with just the prefix applied
				var prefixItem = baseItem.Clone();
				prefixItem.Prefix(item.prefix);

				try
				{
					foreach (var tooltipLine in vanillaTooltips)
					{
						double outNumber = 0d;
						string newTooltipLine = tooltipLine.text;
						Color? newColor = tooltipLine.overrideColor;
						string tooltipEndText =
							new string(tooltipLine.text
								.Reverse()
								.TakeWhile(x => !char.IsDigit(x))
								.Reverse()
								.ToArray());

						//private string[] _prefixTooltipLines = {
						//		"PrefixDamage", "PrefixSpeed", "PrefixCritChance", "PrefixUseMana", "PrefixSize",
						//		"PrefixShootSpeed", "PrefixKnockback", "PrefixAccDefense", "PrefixAccMaxMana",
						//		"PrefixAccCritChance", "PrefixAccDamage", "PrefixAccMoveSpeed", "PrefixAccMeleeSpeed"
						//	};

						if (tooltipLine.Name.Equals("PrefixDamage"))
						{
							newTooltipLine = baseItem.damage > 0
								? GetPrefixNormString(baseItem.damage, prefixItem.damage, ref outNumber, ref newColor)
								: GetPrefixNormString(prefixItem.damage, baseItem.damage, ref outNumber, ref newColor);
						}
						else if (tooltipLine.Name.Equals("PrefixSpeed"))
						{
							newTooltipLine = baseItem.useAnimation <= 0
								? GetPrefixNormString(baseItem.useAnimation, prefixItem.useAnimation, ref outNumber, ref newColor)
								: GetPrefixNormString(prefixItem.useAnimation, baseItem.useAnimation, ref outNumber, ref newColor);
						}
						else if (tooltipLine.Name.Equals("PrefixCritChance"))
						{
							outNumber = prefixItem.crit - baseItem.crit;
							float defColorVal = Main.mouseTextColor / 255f;
							int alphaColor = Main.mouseTextColor;
							newTooltipLine = "";
							if (outNumber >= 0)
							{
								newTooltipLine += "+";
								newColor = new Color((byte)(120f * defColorVal), (byte)(190f * defColorVal), (byte)(120f * defColorVal), alphaColor);
							}
							else
							{
								newColor = new Color((byte)(190f * defColorVal), (byte)(120f * defColorVal), (byte)(120f * defColorVal), alphaColor);
							}

							newTooltipLine += outNumber.ToString(CultureInfo.InvariantCulture);
						}
						else if (tooltipLine.Name.Equals("PrefixUseMana"))
						{
							if (baseItem.mana != 0)
							{
								float defColorVal = Main.mouseTextColor / 255f;
								int alphaColor = Main.mouseTextColor;
								newTooltipLine = GetPrefixNormString(baseItem.mana, prefixItem.mana, ref outNumber, ref newColor);
								newColor = prefixItem.mana < baseItem.mana
									? new Color((byte)(120f * defColorVal), (byte)(190f * defColorVal), (byte)(120f * defColorVal), alphaColor)
									: new Color((byte)(190f * defColorVal), (byte)(120f * defColorVal), (byte)(120f * defColorVal), alphaColor);
							}
						}
						else if (tooltipLine.Name.Equals("PrefixSize"))
						{
							newTooltipLine = baseItem.scale > 0
								? GetPrefixNormString(baseItem.scale, prefixItem.scale, ref outNumber, ref newColor)
								: GetPrefixNormString(prefixItem.scale, baseItem.scale, ref outNumber, ref newColor);
						}
						else if (tooltipLine.Name.Equals("PrefixShootSpeed"))
						{
							newTooltipLine = baseItem.shootSpeed > 0
								? GetPrefixNormString(baseItem.shootSpeed, prefixItem.shootSpeed, ref outNumber, ref newColor)
								: GetPrefixNormString(prefixItem.shootSpeed, baseItem.shootSpeed, ref outNumber, ref newColor);
						}
						else if (tooltipLine.Name.Equals("PrefixKnockback"))
						{
							newTooltipLine = baseItem.knockBack > 0
								? GetPrefixNormString(baseItem.knockBack, prefixItem.knockBack, ref outNumber, ref newColor)
								: GetPrefixNormString(prefixItem.knockBack, baseItem.knockBack, ref outNumber, ref newColor);
						}
						else
						{
							continue;
						}

						int ttlI = tooltips.FindIndex(x => x.mod.Equals(tooltipLine.mod) && x.Name.Equals(tooltipLine.Name));
						if (ttlI == -1)
						{
							continue;
						}

						if (outNumber == 0d)
						{
							tooltips.RemoveAt(ttlI);
						}
						else
						{
							tooltips[ttlI].text = $"{newTooltipLine}{tooltipEndText}";
							tooltips[ttlI].overrideColor = newColor;
						}
					}
				}
				catch (Exception e)
				{
					Loot.Logger.Error(
						$"A problem occurred during modification of the item's tooltip." +
						$"\nItem in question: {item.AffixName()}",
						e);
				}
				// RECALC END

				// Modifies the tooltips, to insert generic mods data
				int i = tooltips.FindIndex(x => x.mod == "Terraria" && x.Name == "ItemName");
				if (i != -1)
				{
					var namelayer = tooltips[i];

					if (Rarity.ItemPrefix != null)
					{
						namelayer.text = $"{Rarity.ItemPrefix} {namelayer.text}";
					}

					if (Rarity.ItemSuffix != null)
					{
						namelayer.text = $"{namelayer.text} {Rarity.ItemSuffix}";
					}

					if (Rarity.OverrideNameColor != null)
					{
						namelayer.overrideColor = Rarity.OverrideNameColor;
					}

					tooltips[i] = namelayer;
				}


				CheatedItemHackGlobalItem cheatedItemHackGlobalItem = CheatedItemHackGlobalItem.GetInfo(item);
				bool isVanityIgnored = cheatedItemHackGlobalItem.ShouldBeIgnored(item, Main.LocalPlayer);
				Color? inactiveColor = isVanityIgnored ? (Color?)Color.DarkSlateGray : null;
				// Insert modifier rarity

				if (!(Rarity is NullModifierRarity))
				{
					i = tooltips.Count;
					tooltips.Insert(i, new TooltipLine(mod, "Loot: Modifier:Rarity", $"[{Rarity.RarityName}]{(isVanityIgnored ? " [IGNORED]" : "")}")
					{
						overrideColor = inactiveColor ?? Rarity.Color * Main.inventoryScale
					});
				}

				i = tooltips.Count - 1;

				foreach (var modifier in pool.Modifiers)
				{
					foreach (var tt in modifier.GetTooltip().Build())
					{
						tooltips.Insert(++i, new TooltipLine(mod, $"Loot: Modifier:Line:{i}", $"{modifier.GetFormattedUniqueName()} {tt.Text}".TrimStart())
						{
							overrideColor = inactiveColor ?? (tt.Color ?? Color.White) * Main.inventoryScale
						});
					}
				}

				if (SealedModifiers)
				{
					var ttl = new TooltipLine(mod, "Loot: Modifier:Sealed", "Modifiers cannot be changed")
					{
						overrideColor = inactiveColor ?? Color.Cyan
					};
					tooltips.Insert(++i, ttl);
				}

				foreach (var e in pool.Modifiers)
				{
					e.ModifyTooltips(item, tooltips);
				}
			}
		}
	}
}
