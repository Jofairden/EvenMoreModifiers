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
		public override ModifierProperties GetModifierProperties(Item item)
		{
			return new ModifierProperties(magnitudeStrength: item.accessory ? .6f : 1f);
		}

		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.accessory || ArmorModifier.IsArmor(ctx.Item);
	}
}