using Loot.Modifiers.WeaponModifiers;
using Loot.System;

namespace Loot.Pools
{
	internal class WeaponModifierPool : ModifierPool
	{
		public WeaponModifierPool()
		{
			Effects = new Modifier[]
			{
				Loot.Instance.GetModifier<DamagePlus>(),
				Loot.Instance.GetModifier<CritPlus>(),
				Loot.Instance.GetModifier<SpeedPlus>(),
				Loot.Instance.GetModifier<KnockbackPlus>(),
				Loot.Instance.GetModifier<VelocityPlus>(),
				Loot.Instance.GetModifier<ManaReduce>(),
				Loot.Instance.GetModifier<AmmoReduce>(),
				Loot.Instance.GetModifier<DamageWithManaCost>(),
				Loot.Instance.GetModifier<DamagePlusDaytime>(),
				Loot.Instance.GetModifier<CursedDamage>(),
				Loot.Instance.GetModifier<MissingHealthDamage>(),
				Loot.Instance.GetModifier<VelocityDamage>(),
				Loot.Instance.GetModifier<RandomDebuff>(),
			};
		}
	}
}
