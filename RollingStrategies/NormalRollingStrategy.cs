using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Api.Strategy;
using Loot.Modifiers.EquipModifiers.Utility;
using Terraria;
using Terraria.Utilities;

namespace Loot.RollingStrategies
{
	/// <summary>
	/// A normal rolling strategy, follows normal behavior
	/// </summary>
	public class NormalRollingStrategy : RollingStrategy<RollingStrategyContext>
	{
		//// Forces the next roll to succeed
		//protected bool _forceNextRoll;

		//protected RollingStrategyProperties properties;
		//protected LootModItem modifierItem;

		//protected virtual IEnumerable<Modifier> GetRollableModifiers(ModifierContext context)
		//	=> modifierItem.ModifierPool.GetRollableModifiers(context);

		//private void InitFields(RollingStrategyContext strategyContext)
		//{
		//	properties = strategyContext.Properties;
		//	modifierItem = LootModItem.GetInfo(strategyContext.Item);
		//}

		//private WeightedRandom<Modifier> GetRandomList(ModifierContext context)
		//{
		//	WeightedRandom<Modifier> wr = new WeightedRandom<Modifier>();
		//	foreach (var e in GetRollableModifiers(context))
		//		wr.Add(e, e.Properties.RollChance);
		//	return wr;
		//}

		//public bool Roll(ModifierContext modifierContext, RollingStrategyContext strategyContext)
		//{
		//	InitFields(strategyContext);

		//	var wr = GetRandomList(modifierContext);
		//	var list = new List<Modifier>();

		//	// Up to n times, try rolling a mod
		//	// TODO since we can increase lines rolled, make it so that MaxRollableLines influences the number of rows drawn in the UI
		//	for (int i = 0; i < properties.MaxRollableLines; ++i)
		//	{
		//		// If there are no mods left, or we fail the roll, break.
		//		if (wr.elements.Count <= 0
		//			|| !_forceNextRoll
		//			&& list.Count >= properties.MinModifierRolls
		//			&& Main.rand.NextFloat() > properties.RollNextChance)
		//		{
		//			break;
		//		}

		//		_forceNextRoll = false;

		//		// Get a next weighted random mod
		//		// Clone the mod (new instance) and roll its properties, then roll it
		//		Modifier rolledModifier = (Modifier)wr.Get().Clone();
		//		float luck = properties.ExtraLuck;
		//		float magnitudePower = properties.MagnitudePower;

		//		if (modifierContext.Player != null)
		//		{
		//			luck += ModifierDelegatorPlayer.GetPlayer(modifierContext.Player).GetEffect<LuckEffect>().Luck;
		//		}
		//		if (modifierContext.Rarity != null)
		//		{
		//			luck += modifierContext.Rarity.ExtraLuck;
		//			magnitudePower += modifierContext.Rarity.ExtraMagnitudePower;
		//		}

		//		rolledModifier.Properties =
		//			rolledModifier.GetModifierProperties(modifierContext.Item)
		//				.Build()
		//				.RollMagnitudeAndPower(magnitudePower, luck);

		//		rolledModifier.Roll(modifierContext, list);

		//		// If the mod deemed to be unable to be added,
		//		// Force that the next roll is successful
		//		// (no RNG on top of RNG)
		//		if (!rolledModifier.PostRoll(modifierContext, list))
		//		{
		//			_forceNextRoll = true;
		//			continue;
		//		}

		//		// The mod can be added
		//		list.Add(rolledModifier);

		//		// If it is a unique modifier, remove it from the list to be rolled
		//		if (rolledModifier.Properties.IsUnique)
		//		{
		//			wr.elements.RemoveAll(x => x.Item1.Type == rolledModifier.Type);
		//			wr.needsRefresh = true;
		//		}
		//	}

		//	modifierItem.ModifierPool.ActiveModifiers = list.ToArray();
		//	return list.Any();
		//}

		public override List<Modifier> Roll(ModifierPool pool, ModifierContext modifierContext, RollingStrategyContext strategyContext)
		{
			var modifiers = new List<Modifier>();
			var rollable = pool.GetRollableModifiers(modifierContext).ToList();
			WeightedRandom<Modifier> wr = new WeightedRandom<Modifier>();
			rollable.ForEach(x => wr.Add(x));

			for (int i = 0; i < 4; i++)
			{
				var rolled = (Modifier)wr.Get().Clone();
				float luck = strategyContext.Properties.ExtraLuck;
				float magnitudePower = strategyContext.Properties.MagnitudePower;

				if (modifierContext.Player != null)
				{
					luck += ModifierDelegatorPlayer.GetPlayer(modifierContext.Player).GetEffect<LuckEffect>().Luck;
				}
				if (modifierContext.Rarity != null)
				{
					luck += modifierContext.Rarity.ExtraLuck;
					magnitudePower += modifierContext.Rarity.ExtraMagnitudePower;
				}
				rolled.Properties =
					rolled.GetModifierProperties(modifierContext.Item)
									.Build()
									.RollMagnitudeAndPower(magnitudePower, luck);

				rolled.Roll(modifierContext, modifiers);
				modifiers.Add(rolled);
			}

			return modifiers;
		}
	}
}
