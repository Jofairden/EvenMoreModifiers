using Loot.Modifiers.AccessoryModifiers;
using Loot.System;

namespace Loot.Pools
{
	internal class AccessoryModifierPool : ModifierPool
	{
		public AccessoryModifierPool()
		{
			Modifiers = new Modifier[]
			{
				Loot.Instance.GetModifier<Inferno>(),
				Loot.Instance.GetModifier<GodlyDefense>(),
				Loot.Instance.GetModifier<MoreDamage>()
			};
		}
	}
}
