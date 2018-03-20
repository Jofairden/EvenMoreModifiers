using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DebuffCursedInfernoEffect : DebuffEffect
	{
		public override string buffName() => "Cursed Inferno";
		public override int buffType() => Terraria.ID.BuffID.CursedInferno;
		public override int buffTime() => 180;
	}
}
