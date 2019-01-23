using Loot.Core.System.Modifier;
using Loot.Ext;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier that can roll on a weapon item
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class ArmorModifier : Modifier
	{
		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsArmor();
	}
}
