using Loot.Effects;
using Loot.Effects.WeaponEffects;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.Ours
{
	internal class WeaponModifier : Modifier
	{
		public WeaponModifier()
		{
			Effects = new ModifierEffect[]
			{
				/*Loot.Instance.GetModifierEffect<DamagePlusEffect>(),
				Loot.Instance.GetModifierEffect<CritPlusEffect>(),
				Loot.Instance.GetModifierEffect<SpeedPlusEffect>(),
				Loot.Instance.GetModifierEffect<KnockbackPlusEffect>(),
				Loot.Instance.GetModifierEffect<VelocityPlusEffect>(),*/
				Loot.Instance.GetModifierEffect<ManaReduceEffect>(),
				Loot.Instance.GetModifierEffect<AmmoReduceEffect>()//,
				/*Loot.Instance.GetModifierEffect<DamageWithManaCostEffect>(),
				Loot.Instance.GetModifierEffect<DamagePlusDayEffect>(),
				Loot.Instance.GetModifierEffect<DamagePlusNightEffect>(),
				Loot.Instance.GetModifierEffect<CursedDamageEffect>(),
				Loot.Instance.GetModifierEffect<MissingHealthDamageEffect>(),
				Loot.Instance.GetModifierEffect<VelocityDamageEffect>(),
				Loot.Instance.GetModifierEffect<DebuffPoisonEffect>(),
				Loot.Instance.GetModifierEffect<DebuffFireEffect>(),
				Loot.Instance.GetModifierEffect<DebuffFrostburnEffect>(),
				Loot.Instance.GetModifierEffect<DebuffCursedInfernoEffect>(),
				Loot.Instance.GetModifierEffect<DebuffConfuseEffect>(),
				Loot.Instance.GetModifierEffect<DebuffIchorEffect>()*/
			};
		}
		
		private bool WeaponCheck(ModifierContext ctx) => ctx.Item.damage > 0 && ctx.Item.useTime > 0 && ctx.Item.maxStack == 1;

		public override bool CanApplyReforge(ModifierContext ctx) => WeaponCheck(ctx);
		public override bool CanApplyCraft(ModifierContext ctx) => WeaponCheck(ctx);
		public override bool CanApplyPickup(ModifierContext ctx) => WeaponCheck(ctx);
	}
}
