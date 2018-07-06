using Loot.Core;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier that can roll on an accessory item
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class AccessoryModifier : Modifier
	{
		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsAccessory();
	}
}
