using Loot.Api.Core;
using Loot.Api.Ext;
using Terraria;

namespace Loot.Modifiers.Base
{
	/// <summary>
	/// Defines a modifier that can roll on a weapon item
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class WeaponModifier : Modifier
	{
		public static bool HasVanillaDamage(Item item)
			=> item.magic || item.melee || item.ranged || item.summon || item.thrown;

		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsWeapon();
	}
}
