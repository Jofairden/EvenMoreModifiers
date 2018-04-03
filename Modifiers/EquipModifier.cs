using System;
using Loot.System;
using Terraria;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier that can roll on an accessory item or an armor item
	/// These modifiers will have 60% maximum Power on accessories
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class EquipModifier : Modifier
	{
		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.accessory || (ctx.Item.headSlot > 0 || ctx.Item.legSlot > 0 || ctx.Item.bodySlot > 0) && !ctx.Item.vanity;

		public override void Apply(Item item)
		{
			if (item.accessory)
				Power = BasePower * ((Magnitude - MinMagnitude) * 0.6f + MinMagnitude);
		}
	}
}