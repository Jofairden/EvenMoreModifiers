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

		internal void RollNewModifier(ModifierContext ctx)
		{
			Modifier = EMMLoader.GetWeightedModifier(ctx);
			if (Modifier != null)
			{
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

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			EMMItem clone = (EMMItem)base.Clone(item, itemClone);
			clone.Modifier = (Modifier)Modifier?.Clone();
			return clone;
		}

		public override void Load(Item item, TagCompound tag)
		{
			GetItemInfo(item).Modifier = Modifier._Load(tag);
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
			if (m == null)
				GetItemInfo(item)?.RollNewModifier(ctx);

			m = GetModifier(item);
			m?.ApplyItem(ctx);
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
