using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DebuffPoisonEffect : DebuffEffect
	{
		public override string buffName() => "Poison";
		public override int buffType() => Terraria.ID.BuffID.Poisoned;
		public override int buffTime() => 480;
	}
}
