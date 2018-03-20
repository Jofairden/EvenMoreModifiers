using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Effects.WeaponEffects
{
	public class DamagePlusEffect : ModifierEffect
	{
		public DamagePlusEffect()
		{

		}

		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.damage = (int)Math.Ceiling(ctx.Item.damage * (1 + Power / 100f));
		}
	}

	public class CritPlusEffect : ModifierEffect
	{
		public CritPlusEffect()
		{

		}

		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% crit chance", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.crit += (int)Math.Round(Power);
		}
	}

	public class SpeedPlusEffect : ModifierEffect
	{
		public SpeedPlusEffect()
		{

		}

		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% speed", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.1f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 10f;

		public override void ApplyItem(ModifierContext ctx)
		{
			// Floor the effect so that it will always increase speed by at least 1 frame
			ctx.Item.useTime = (int)Math.Floor((float)ctx.Item.useTime * (1 - Power / 100f));
			ctx.Item.useAnimation = (int)Math.Floor((float)ctx.Item.useAnimation * (1 - Power / 100f));

			// Don't go below the minimum
			if (ctx.Item.useTime < 2) ctx.Item.useTime = 2;
			if (ctx.Item.useAnimation < 2) ctx.Item.useAnimation = 2;
		}
	}

	public class KnockbackPlusEffect : ModifierEffect
	{
		public KnockbackPlusEffect()
		{

		}

		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% knockback", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.05f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 20f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.knockBack *= Power / 100 + 1;
		}
	}

	public class VelocityPlusEffect : ModifierEffect
	{
		public VelocityPlusEffect()
		{

		}

		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% velocity", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.05f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 20f;

		public override bool CanRoll(ModifierContext ctx)
		{
			// Only apply on items that shoot projectiles
			return ctx.Item.shoot > 0;
		}

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.shootSpeed *= Power / 100 + 1;
		}
	}

	public class ManaReduceEffect : ModifierEffect
	{
		public ManaReduceEffect()
		{

		}

		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"-{(int)Math.Round(Power)}% mana cost", Color = Color.Lime}
			};

		public override float MinMagnitude => (float)1/15;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 15f;

		public override bool CanRoll(ModifierContext ctx)
		{
			// Only apply on items with a mana cost
			return ctx.Item.mana > 0;
		}

		public override void ApplyItem(ModifierContext ctx)
		{
			// Always reduce by at least 1 mana cost
			ctx.Item.mana = (int)Math.Floor(ctx.Item.mana * (1 - Power / 100f));

			// Don't go below 1 mana cost! 0 cost is too OP :P
			if (ctx.Item.mana < 1) ctx.Item.mana = 1;
		}
	}

	public class AmmoReduceEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"{(int)Math.Round(Power)}% chance to not consume ammo", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.05f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 20f;

		public override bool CanRoll(ModifierContext ctx)
		{
			// Only apply on items that consume ammo
			return ctx.Item.useAmmo > 0;
		}

		public override void HoldItem(ModifierContext ctx)
		{
			EMMItem.GetItemInfo(ctx.Item).dontConsumeAmmo = Power/100;
		}
	}

	public class DamageWithManaCostEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage, but added mana cost", Color = Color.Lime}
			};

		public override float MinMagnitude => 16f/30;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 30f;

		public override bool CanRoll(ModifierContext ctx)
		{
			// Only roll on items that don't already cost mana
			return ctx.Item.mana == 0;
		}

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.damage = (int)Math.Ceiling(ctx.Item.damage * (1 + Power / 100f));
			ctx.Item.mana = Math.Max((int)(25 * (ctx.Item.useTime / 60.0)), 1);
		}
	}

	public class DamagePlusDayEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage during the day", Color = Color.Lime}
			};

		public override float MinMagnitude => 1.0f/15;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 15f;

		public override void UpdateItem(ModifierContext ctx, bool isItemEquipped = false)
		{
			EMMItem.GetItemInfo(ctx.Item).dayDamageBonus = Power;
		}
	}

	public class DamagePlusNightEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage during the night", Color = Color.Lime}
			};

		public override float MinMagnitude => 1.0f / 15;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 15f;

		public override void UpdateItem(ModifierContext ctx, bool isItemEquipped = false)
		{
			EMMItem.GetItemInfo(ctx.Item).nightDamageBonus = Power;
		}
	}

	public class CursedDamageEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% damage, but cursed", Color = Color.Lime}
			};

		public override float MinMagnitude => 16f/30;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 30f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.damage = (int)Math.Ceiling(ctx.Item.damage * (1 + Power / 100f));
		}

		public override void HoldItem(ModifierContext ctx)
		{
			EMMPlayer.PlayerInfo(ctx.Player).holdingCursed = true;
		}
	}

	public class MissingHealthDamageEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"Up to +{(int)Math.Round(Power*6)}% damage based on missing health", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.2f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 5f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			EMMItem.GetItemInfo(ctx.Item).missingHealthBonus = Power;
		}
	}

	public class VelocityDamageEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"Added damage based on velocity (multiplier: {Math.Round(Power/2, 1)}x)", Color = Color.Lime}
			};

		public override float MinMagnitude => 1f/7;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 7f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			EMMItem.GetItemInfo(ctx.Item).velocityDamageBonus = Power;
		}
	}

	// This class makes all the debuff effects easier :P
	// Also makes it easy to add more debuff effects. Midas effect anyone?
	public abstract class DebuffEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
		{
			new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% chance to inflict {buffName()} for {buffTime()/60f}s", Color = Color.Lime }
		};

		public override float MinMagnitude => 0.02f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 50f;

		public abstract string buffName();
		public abstract int buffType();
		public abstract int buffTime();

		public override void HoldItem(ModifierContext ctx)
		{
			EMMPlayer.PlayerInfo(ctx.Player).debuffChances.Add(new Tuple<float, int, int>(Power/100, buffType(), buffTime()));
		}
	}
	public class DebuffPoisonEffect : DebuffEffect
	{
		public override string buffName() => "Poison";
		public override int buffType() => Terraria.ID.BuffID.Poisoned;
		public override int buffTime() => 480;
	}
	public class DebuffFireEffect : DebuffEffect
	{
		public override string buffName() => "On Fire!";
		public override int buffType() => Terraria.ID.BuffID.OnFire;
		public override int buffTime() => 300;
	}
	public class DebuffFrostburnEffect : DebuffEffect
	{
		public override string buffName() => "Frostburn";
		public override int buffType() => Terraria.ID.BuffID.Frostburn;
		public override int buffTime() => 240;
	}
	public class DebuffConfuseEffect : DebuffEffect
	{
		public override string buffName() => "Confusion";
		public override int buffType() => Terraria.ID.BuffID.Confused;
		public override int buffTime() => 120;
	}
	public class DebuffCursedInfernoEffect : DebuffEffect
	{
		public override string buffName() => "Cursed Inferno";
		public override int buffType() => Terraria.ID.BuffID.CursedInferno;
		public override int buffTime() => 180;
	}
	public class DebuffIchorEffect : DebuffEffect
	{
		public override float MinMagnitude => 0.05f;
		public override float BasePower => 20f;

		public override string buffName() => "Ichor";
		public override int buffType() => Terraria.ID.BuffID.Ichor;
		public override int buffTime() => 180;
	}
}
