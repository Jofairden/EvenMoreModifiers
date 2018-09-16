using Loot.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	// TODO , this could should be added to to include min/max magnitude rolls, that influence the time and chance to apply 
	public struct DebuffTrigger
	{
		public int BuffType;
		public int BuffTime;
		public float InflictionChance;

		public DebuffTrigger(int buffType, int buffTime, float inflictionChance)
		{
			BuffType = buffType;
			BuffTime = buffTime;
			InflictionChance = inflictionChance;
		}
	}

	public class WeaponDebuffEffect : ModifierEffect
	{
		// List of current debuff chances. Tuple format is [chance, buffType, buffTime]
		// TODO with c#7 we should favor a named tuple (waiting for TML support)
		//public IList<(float chance, int type, int time)> DebuffChances;
		public IList<DebuffTrigger> DebuffChances = new List<DebuffTrigger>();

		public override void OnInitialize(ModifierPlayer player)
		{
			DebuffChances = new List<DebuffTrigger>();
		}

		public override void ResetEffects(ModifierPlayer player)
		{
			DebuffChances?.Clear();
		}
	}

	/// <summary>
	/// Defines a modifier that can inflict a debuff, applicable for weapons
	/// </summary>
	[UsesEffect(typeof(WeaponDebuffEffect))] // The attribute is inherited, which is nice
	public abstract class WeaponDebuffModifier : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {Text = $"+{Properties.RoundedPower}% chance to inflict {GetBuffName()} for {BuffTime / 60f}s", Color = Color.Lime}
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: Math.Max(5, Math.Min(20, item.damage)), maxMagnitude: item.damage > 800 ? 100f : 50f);
		}

		public abstract int BuffType { get; }
		public abstract int BuffTime { get; }
		public abstract float BuffInflictionChance { get; }

		private string GetBuffName()
		{
			if (BuffType >= BuffID.Count)
			{
				return BuffLoader.GetBuff(BuffType)?.DisplayName.GetTranslation(LanguageManager.Instance.ActiveCulture) ?? "null";
			}

			return Lang.GetBuffName(BuffType);
		}

		public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
		{
			if (Main.rand.NextFloat() < Properties.RoundedPower / 100f * BuffInflictionChance)
			{
				target.AddBuff(BuffType, BuffTime);
			}
		}

		public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
		{
			if (Main.rand.NextFloat() < Properties.RoundedPower / 100f * BuffInflictionChance)
			{
				target.AddBuff(BuffType, BuffTime);
			}
		}

		// Required for minion/proj snapshotting
		public override void HoldItem(Item item, Player player)
		{
			ModifierPlayer.Player(player).GetEffect<WeaponDebuffEffect>()
				.DebuffChances
				.Add(new DebuffTrigger(BuffType, BuffTime, Properties.RoundedPower / 100f * BuffInflictionChance));
		}
	}
}