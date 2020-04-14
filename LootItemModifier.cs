using System.Collections.Generic;
using Loot.Api.Core;
using Loot.Api.Mechanism;
using Loot.Api.Strategy;
using Terraria;

namespace Loot
{
	static class LootItemModifier
	{
		public static Item RerollModifiers(this Item item, ModifierContext context, RollingStrategyProperties properties)
		{
			var refItem = item.Clone();
			var pool = ModifierPoolMechanism.GetPool(context);
			var selected = ModifierMechanism.RollModifiers(item, pool, context, properties);
			return refItem.UpdateModifiers(selected);
		}

		public static Item UpdateModifiers(this Item item, List<Modifier> modifiers)
		{
			LootModItem.GetInfo(item).Modifiers = new FiniteModifierPool(modifiers);
			LootModItem.GetInfo(item).Modifiers.Modifiers.ForEach(x =>
			{
				x.Apply(item);
			});
			return item;
		}
	}
}
