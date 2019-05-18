using Loot.Api.Ext;
using Loot.Api.Modifier;

namespace Loot.Modifiers.Base
{
	/// <summary>
	/// Defines a modifier that can roll on an armor item (head/body/legs)
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class ArmorModifier : Modifier
	{
		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsArmor();
	}
}
