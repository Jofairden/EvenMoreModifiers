using System;
using Microsoft.Xna.Framework;
using Loot.Modifiers;

namespace Loot.Effects.WeaponEffects
{
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
			ModifierPlayer.PlayerInfo(ctx.Player).debuffChances.Add(new Tuple<float, int, int>(Power / 100, buffType(), buffTime()));
		}
	}
}
