using System;
using System.Collections.Generic;
using Loot.Api.Attributes;
using Loot.Api.Delegators;
using Loot.Api.Modifier;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Loot.Modifiers.Base
{
	public class WeaponDebuffEffect : ModifierEffect
	{
		public List<(int type, int time, float chance)> DebuffChances = new List<(int type, int time, float chance)>();

		public override void OnInitialize()
		{
			DebuffChances = new List<(int type, int time, float chance)>();
		}

		public override void ResetEffects()
		{
			DebuffChances.Clear();
		}
	}

	/// <summary>
	/// Defines a modifier that can inflict a debuff, applicable for weapons
	/// </summary>
	[UsesEffect(typeof(WeaponDebuffEffect))] // The attribute is inherited, which is nice
	public abstract class WeaponDebuffModifier : WeaponModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% chance to inflict {GetBuffName()} for {RoundedBuffTime()}s");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMinMagnitude(5f)
				.WithMinMagnitude(10f)
				.WithRoundPrecision(1);
		}

		private float RoundedBuffTime()
		{
			return (float) Math.Round((double) BuffTime / 60f, Properties.RoundPrecision);
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
			ModifierDelegatorPlayer.GetPlayer(player).GetEffect<WeaponDebuffEffect>()
				.DebuffChances
				.Add((BuffType, BuffTime, Properties.RoundedPower / 100f * BuffInflictionChance));
		}
	}
}
