using Loot.Core.Cubes;
using Loot.Core.Graphics;
using Loot.Core.System;
using Loot.Core.System.Loaders;
using Loot.Ext;
using Loot.Modifiers.EquipModifiers.Utility;
using Loot.Pools;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Loot
{
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

		/// <summary>
		/// Attempts to roll new modifiers
		/// Has a set chance to hit a predefined pool of modifiers
		/// </summary>
		internal ModifierPool RollNewPool(ModifierContext ctx, ItemRollProperties itemRollProperties = null)
		{
			// Default to normal
			if (itemRollProperties == null)
			{
				itemRollProperties = new ItemRollProperties();
			}

			// Now we have actually rolled
			HasRolled = true;

			// Rolling logic
			bool noForce = true;

			// Custom roll behavior provided
			if (itemRollProperties.OverrideRollModifierPool != null)
			{
				ModifierPool = itemRollProperties.OverrideRollModifierPool.Invoke();
				noForce = !ModifierPool?._CanRoll(ctx) ?? true;
			}

			// No behavior provided
			if (noForce)
			{
				// A pool is forced to roll
				if (itemRollProperties.ForceModifierPool != null)
				{
					ModifierPool = ContentLoader.ModifierPool.GetContent(itemRollProperties.ForceModifierPool.GetType());
					noForce = !ModifierPool?._CanRoll(ctx) ?? true;
				}

				// No pool forced to roll or it's not valid
				if (noForce)
				{
					// Try rolling a predefined (weighted) pool
					bool rollPredefinedPool = Main.rand.NextFloat() <= itemRollProperties.RollPredefinedPoolChance;
					noForce = !rollPredefinedPool;

					if (rollPredefinedPool)
					{
						// GetWeightedPool already checks _CanRoll
						ModifierPool = ContentLoader.ModifierPool.GetWeightedPool(ctx);
						noForce = ModifierPool == null || !ModifierPool._CanRoll(ctx);
					}

					// Roll from all modifiers
					if (noForce)
					{
						ModifierPool = Loot.Instance.GetModifierPool<AllModifiersPool>();
						if (!ModifierPool._CanRoll(ctx))
						{
							ModifierPool = null;
							return null;
						}
					}
				}
			}

			// Attempt rolling modifiers
			if (!RollNewModifiers(ctx, itemRollProperties))
			{
				ModifierPool = null; // reset (didn't roll anything)
			}
			else
			{
				ModifierPool.UpdateRarity();
			}

			ctx.Item.GetGlobalItem<ShaderGlobalItem>().NeedsUpdate = true;
			ctx.Item.GetGlobalItem<GlowmaskGlobalItem>().NeedsUpdate = true;
			return ModifierPool;
		}

		// Forces the next roll to succeed
		private bool _forceNextRoll;

		/// <summary>
		/// Roll active modifiers, can roll up to n maximum effects
		/// Returns if any modifiers were activated
		/// </summary>
		internal bool RollNewModifiers(ModifierContext ctx, ItemRollProperties itemRollProperties)
		{
			// Firstly, prepare a WeightedRandom list with modifiers
			// that are rollable in this context
			WeightedRandom<Modifier> wr = new WeightedRandom<Modifier>();
			List<Modifier> list = new List<Modifier>();

			foreach (var e in ModifierPool.GetRollableModifiers(ctx))
			{
				wr.Add(e, e.Properties.RollChance);
			}

			// Up to n times, try rolling a mod
			// @ todo since we can increase lines rolled, make it so that MaxRollableLines influences the number of rows drawn in the UI
			for (int i = 0; i < itemRollProperties.MaxRollableLines; ++i)
			{
				// If there are no mods left, or we fail the roll, break.
				if (wr.elements.Count <= 0
					|| !_forceNextRoll
					&& i > 0
					&& list.Count >= itemRollProperties.MinModifierRolls
					&& Main.rand.NextFloat() > itemRollProperties.RollNextChance)
				{
					break;
				}

				_forceNextRoll = false;

				// Get a next weighted random mod
				// Clone the mod (new instance) and roll it's properties, then roll it
				Modifier e = wr.Get();
				Modifier eClone = (Modifier)e.Clone();
				float luck = itemRollProperties.ExtraLuck;
				if (ctx.Player != null)
				{
					luck += ModifierPlayer.Player(ctx.Player).GetEffect<LuckEffect>().Luck;
				}
				eClone.Properties =
					eClone.GetModifierProperties(ctx.Item).Build()
					.RollMagnitudeAndPower(
							magnitudePower: itemRollProperties.MagnitudePower,
							lukStat: luck);
				eClone.Roll(ctx, list);

				// If the mod deemed to be unable to be added,
				// Force that the next roll is successful
				// (no RNG on top of RNG)
				if (!eClone.PostRoll(ctx, list))
				{
					_forceNextRoll = true;
					continue;
				}

				// The mod can be added
				list.Add(eClone);

				// If it is a unique modifier, remove it from the list to be rolled
				if (eClone.Properties.IsUnique)
				{
					wr.elements.RemoveAll(x => x.Item1.Type == eClone.Type);
					wr.needsRefresh = true;
				}
			}

			ModifierPool.ActiveModifiers = list.ToArray();
			return list.Any();
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
				{
					ModifierPool = null;
				}
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
			{
				ModifierPool = ModifierPool._NetReceive(item, reader);
			}

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
			{
				ModifierPool._NetSend(ModifierPool, item, writer);
			}

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
					Loot.Instance.Logger.Error(
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
					if (pool.Rarity.ItemPrefix != null)
					{
						namelayer.text = $"{pool.Rarity.ItemPrefix} {namelayer.text}";
					}

					if (pool.Rarity.ItemSuffix != null)
					{
						namelayer.text += $" {pool.Rarity.ItemSuffix}";
					}

					if (pool.Rarity.OverrideNameColor != null)
					{
						namelayer.overrideColor = pool.Rarity.OverrideNameColor;
					}

					tooltips[i] = namelayer;
				}

				// Insert modifier rarity
				ActivatedModifierItem activatedModifierItem = ActivatedModifierItem.Item(item);
				bool isVanityIgnored = activatedModifierItem.ShouldBeIgnored(item, Main.LocalPlayer);

				Color? inactiveColor = isVanityIgnored ? (Color?)Color.DarkSlateGray : null;

				i = tooltips.Count;
				tooltips.Insert(i, new TooltipLine(mod, "Loot: Modifier:Rarity", $"[{pool.Rarity.RarityName}]{(isVanityIgnored ? " [IGNORED]" : "")}") { overrideColor = inactiveColor ?? pool.Rarity.Color * Main.inventoryScale });

				// Insert lines
				foreach (var ttcol in pool.Description)
				{
					foreach (var tt in ttcol)
					{
						tooltips.Insert(++i, new TooltipLine(mod, $"Loot: Modifier:Line:{i}", tt.Text) { overrideColor = inactiveColor ?? (tt.Color ?? Color.White) * Main.inventoryScale });
					}
				}

				// Insert sealed notation
				if (SealedModifiers)
				{
					var ttl = new TooltipLine(mod, "Loot: Modifier:Sealed", "Modifiers cannot be changed")
					{
						overrideColor = inactiveColor ?? Color.Cyan
					};
					tooltips.Insert(++i, ttl);
				}

				// Call modify tooltips
				foreach (var e in pool.ActiveModifiers)
				{
					e.ModifyTooltips(item, tooltips);
				}
			}
		}
	}
}
