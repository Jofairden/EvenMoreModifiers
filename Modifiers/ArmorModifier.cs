using Loot.System;
using Terraria;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier that can roll on a weapon item
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class ArmorModifier : Modifier
	{
		// TODO put useful fields on item itself
		public static bool IsArmor(Item item)
			=> (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1) && !item.vanity;

		public override bool CanRoll(ModifierContext ctx)
			=> IsArmor(ctx.Item);
	}
}
