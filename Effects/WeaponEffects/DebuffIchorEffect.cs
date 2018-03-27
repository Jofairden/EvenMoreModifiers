using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DebuffIchorEffect : DebuffEffect
	{
		public override float MinMagnitude => 0.05f;
		public override float BasePower => 20f;

		public override string buffName() => "Ichor";
		public override int buffType() => Terraria.ID.BuffID.Ichor;
		public override int buffTime() => 180;
	}
}
