using Loot.Core;
using Loot.Core.Attributes;

namespace Loot.Pools
{
	[PopulatePoolFrom("Loot.Modifiers.WeaponModifiers")]
	internal class WeaponModifierPool : ModifierPool
	{
		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsWeapon();
	}
}
