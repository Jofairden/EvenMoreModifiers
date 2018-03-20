using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DebuffConfuseEffect : DebuffEffect
	{
		public override string buffName() => "Confusion";
		public override int buffType() => Terraria.ID.BuffID.Confused;
		public override int buffTime() => 120;
	}
}
