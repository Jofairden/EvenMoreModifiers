using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DebuffFireEffect : DebuffEffect
	{
		public override string buffName() => "On Fire!";
		public override int buffType() => Terraria.ID.BuffID.OnFire;
		public override int buffTime() => 300;
	}
}
