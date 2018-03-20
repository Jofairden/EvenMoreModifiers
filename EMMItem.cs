using System;
using System.Collections.Generic;
using System.IO;
using Loot.Modifiers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot
{
	/// <summary>
	/// Holds entity based data
	/// </summary>
	public sealed class EMMItem : GlobalItem
	{
		public static EMMItem GetItemInfo(Item item) => item.GetGlobalItem<EMMItem>();
		public static Modifier GetModifier(Item item) => GetItemInfo(item)?.Modifier ?? null;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public Modifier Modifier;

		// Special item parameters
		public bool hasRolled;             // whether this item has rolled a modifier TODO: Save this
		public float dontConsumeAmmo;      // % chance to not consume ammo
		public float dayDamageBonus;       // % damage bonus during the day
		public float nightDamageBonus;     // % damage bonus during the night
		public float missingHealthBonus;   // damage bonus scale from missing health
		public float velocityDamageBonus;  // damage bonus scale from velocity

		public override bool ConsumeAmmo(Item item, Player player)
		{
			// Ammo consumption chance
			return Main.rand.NextFloat() > dontConsumeAmmo;
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			if (Main.dayTime && dayDamageBonus > 0) damage = (int)Math.Ceiling(damage * (1 + dayDamageBonus / 100));
			if (!Main.dayTime && dayDamageBonus > 0) damage = (int)Math.Ceiling(damage * (1 + nightDamageBonus / 100));
			if (missingHealthBonus > 0)
			{
				// Formula ported from old mod
				float mag = (missingHealthBonus * ((player.statLifeMax2 - player.statLife) / (float)player.statLifeMax2) * 6);
				damage = (int)(damage * (1 + mag / 100));
			}
			if (velocityDamageBonus > 0 && player.velocity.Length() > 0)
			{
				// Formula ported from old mod
				float magnitude = velocityDamageBonus * player.velocity.Length() / 4;
				damage = (int)(damage * (1 + magnitude / 100));
			}
		}

		internal void RollNewModifier(ModifierContext ctx)
		{
			if (hasRolled) return;
			
			Modifier = EMMLoader.GetWeightedModifier(ctx);
			if (Modifier != null)
			{
				hasRolled = true;
				if (Modifier.RollEffects(ctx).Length <= 0)
					Modifier = null;
				else
				{
					foreach (var e in Modifier.ActiveEffects)
						e.RollAndApplyMagnitude();
					Modifier.UpdateRarity();
				}
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			ModifierContext ctx = new ModifierContext
			{
				Player = player,
				Item = item,
				Method = ModifierContextMethod.UpdateItem
			};

			Modifier?.UpdateItem(ctx);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierContext ctx = new ModifierContext
			{
				Player = player,
				Item = item,
				Method = ModifierContextMethod.UpdateItem
			};

			Modifier?.UpdateItem(ctx, true);
		}

		public override void HoldItem(Item item, Player player)
		{
			ModifierContext ctx = new ModifierContext
			{
				Player = player,
				Item = item,
				Method = ModifierContextMethod.HoldItem
			};

			Modifier?.HoldItem(ctx);
		}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			EMMItem clone = (EMMItem)base.Clone(item, itemClone);
			clone.Modifier = (Modifier)Modifier?.Clone();
			return clone;
		}

		public override void Load(Item item, TagCompound tag)
		{
			GetItemInfo(item).Modifier = Modifier._Load(tag);

			// Apply on load. DUH.
			ModifierContext ctx = new ModifierContext
			{
				Method = ModifierContextMethod.OnPickup,
				Item = item
			};
			GetItemInfo(item).Modifier.ApplyItem(ctx);
		}

		public override TagCompound Save(Item item)
		{
			return Modifier.Save(GetItemInfo(item).Modifier);
		}

		public override bool NeedsSaving(Item item) => Modifier != null;

		public override void NetReceive(Item item, BinaryReader reader)
		{
			GetItemInfo(item).Modifier = Modifier._Load(TagIO.FromStream(reader.BaseStream));
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			TagIO.ToStream(Modifier.Save(GetItemInfo(item).Modifier), writer.BaseStream);
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			ModifierContext ctx = new ModifierContext { Method = ModifierContextMethod.OnCraft, Item = item, Player = Main.LocalPlayer };

			Modifier m = GetModifier(item);
			if (m == null)
				GetItemInfo(item)?.RollNewModifier(ctx);

			m = GetModifier(item);
			m?.ApplyItem(ctx);
			base.OnCraft(item, recipe);
		}

		public override bool OnPickup(Item item, Player player)
		{
			ModifierContext ctx = new ModifierContext { Method = ModifierContextMethod.OnPickup, Item = item, Player = player };

			Modifier m = GetModifier(item);
			if (!hasRolled && m == null)
			{
				RollNewModifier(ctx);

				m = GetModifier(item);
				m?.ApplyItem(ctx);
			}
			return base.OnPickup(item, player);
		}

		public override void PostReforge(Item item)
		{
			ModifierContext ctx = new ModifierContext { Method = ModifierContextMethod.OnReforge, Item = item, Player = Main.LocalPlayer };

			Modifier m = GetModifier(item);
			if (m == null)
				GetItemInfo(item)?.RollNewModifier(ctx);

			m = GetModifier(item);
			m?.ApplyItem(ctx);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			var m = GetModifier(item);
			if (m != null)
			{
				int i = tooltips.FindIndex(x => x.mod == "Terraria" && x.Name == "ItemName");
				if (i != -1)
				{
					var namelayer = tooltips[i];
					if (m.Rarity.ItemPrefix != null)
						namelayer.text = $"{m.Rarity.ItemPrefix} {namelayer.text}";
					if (m.Rarity.ItemSuffix != null)
						namelayer.text += $" {m.Rarity.ItemSuffix}";
					if (m.Rarity.OverrideNameColor != null)
						namelayer.overrideColor = m.Rarity.OverrideNameColor;
					tooltips[i] = namelayer;
				}

				i = tooltips.Count;
				tooltips.Insert(i, new TooltipLine(mod, "Modifier:Name", $"[{m.Rarity.Name}]") { overrideColor = m.Rarity.Color });

				foreach (var ttcol in m.Description)
					foreach (var tt in ttcol)
						tooltips.Insert(++i, new TooltipLine(mod, $"Modifier:Description:{i}", tt.Text) { overrideColor = tt.Color ?? Color.White });

				foreach (var e in m.ActiveEffects)
					e.ModifyTooltips(item, tooltips);
			}
		}
	}

}
