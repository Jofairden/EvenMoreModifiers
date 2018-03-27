using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
	public class DebuffFrostburnEffect : DebuffEffect
	{
		public override string buffName() => "Frostburn";
		public override int buffType() => Terraria.ID.BuffID.Frostburn;
		public override int buffTime() => 240;
	}
}
