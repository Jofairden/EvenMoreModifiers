using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Strategy;
using Loot.Sounds;
using Terraria;

namespace Loot.RollingStrategies
{
	public sealed class SealingRollingStrategy : RollingStrategy
	{
		public override List<Modifier> PreRoll(ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			return LootModItem.GetActivePool(modifierContext.Item);
		}

		public override List<Modifier> Roll(List<Modifier> currentModifiers, ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			properties.MinModifierRolls = 0;
			properties.MaxRollableLines = 0;
			return base.Roll(currentModifiers, drawPool, modifierContext, properties);
		}

		public override void PostRoll(ref List<Modifier> modifiers, ModifierPool drawPool, ModifierContext modifierContext, RollingStrategyProperties properties)
		{
			var info = LootModItem.GetInfo(modifierContext.Item);
			info.SealedModifiers = !info.SealedModifiers;
		}

		public override void PlaySoundEffect(Item item)
		{
			SoundHelper.PlayCustomSound(LootModItem.GetInfo(item).SealedModifiers ? SoundHelper.SoundType.GainSeal : SoundHelper.SoundType.LoseSeal);
		}
	}
}
