using System.Collections.Generic;
using System.Linq;
using Loot.Core.System.Modifier;
using Loot.Modifiers.EquipModifiers.Utility;
using Terraria;
using Terraria.Utilities;

namespace Loot.Core.System.Strategy
{
	/// <summary>
	/// A normal rolling strategy, follows normal behavior
	/// </summary>
	public class NormalRollingStrategy : IRollingStrategy<RollingStrategyContext>
	{
		// Forces the next roll to succeed
		protected bool _forceNextRoll;

		protected RollingStrategyProperties properties;
		protected EMMItem modifierItem;

		protected virtual IEnumerable<Modifier.Modifier> GetRollableModifiers(ModifierContext context)
			=> modifierItem.ModifierPool.GetRollableModifiers(context);

		private void InitFields(RollingStrategyContext strategyContext)
		{
			properties = strategyContext.Properties;
			modifierItem = EMMItem.GetItemInfo(strategyContext.Item);
		}

		private WeightedRandom<Modifier.Modifier> GetRandomList(ModifierContext context)
		{
			WeightedRandom<Modifier.Modifier> wr = new WeightedRandom<Modifier.Modifier>();
			foreach (var e in GetRollableModifiers(context))
				wr.Add(e, e.Properties.RollChance);
			return wr;
		}

		public bool Roll(ModifierContext modifierContext, RollingStrategyContext strategyContext)
		{
			InitFields(strategyContext);

			var wr = GetRandomList(modifierContext);
			var list = new List<Modifier.Modifier>();

			// Up to n times, try rolling a mod
			// TODO since we can increase lines rolled, make it so that MaxRollableLines influences the number of rows drawn in the UI
			for (int i = 0; i < properties.MaxRollableLines; ++i)
			{
				// If there are no mods left, or we fail the roll, break.
				if (wr.elements.Count <= 0
					|| !_forceNextRoll
					&& list.Count >= properties.MinModifierRolls
					&& Main.rand.NextFloat() > properties.RollNextChance)
				{
					break;
				}

				_forceNextRoll = false;

				// Get a next weighted random mod
				// Clone the mod (new instance) and roll its properties, then roll it
				Modifier.Modifier rolledModifier = (Modifier.Modifier)wr.Get().Clone();
				float luck = properties.ExtraLuck;
				float magnitudePower = properties.MagnitudePower;

				if (modifierContext.Player != null)
				{
					luck += ModifierPlayer.Player(modifierContext.Player).GetEffect<LuckEffect>().Luck;
				}
				if (modifierContext.Rarity != null)
				{
					luck += modifierContext.Rarity.ExtraLuck;
					magnitudePower += modifierContext.Rarity.ExtraMagnitudePower;
				}

				rolledModifier.Properties =
					rolledModifier.GetModifierProperties(modifierContext.Item).Build()
						.RollMagnitudeAndPower(
							magnitudePower: magnitudePower,
							lukStat: luck);
				rolledModifier.Roll(modifierContext, list);

				// If the mod deemed to be unable to be added,
				// Force that the next roll is successful
				// (no RNG on top of RNG)
				if (!rolledModifier.PostRoll(modifierContext, list))
				{
					_forceNextRoll = true;
					continue;
				}

				// The mod can be added
				list.Add(rolledModifier);

				// If it is a unique modifier, remove it from the list to be rolled
				if (rolledModifier.Properties.IsUnique)
				{
					wr.elements.RemoveAll(x => x.Item1.Type == rolledModifier.Type);
					wr.needsRefresh = true;
				}
			}

			modifierItem.ModifierPool.ActiveModifiers = list.ToArray();
			return list.Any();
		}
	}
}
