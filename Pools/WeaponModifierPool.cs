using Loot.Api.Attributes;
using Loot.Api.Ext;
using Loot.Api.Modifier;

namespace Loot.Pools
{
	[PopulatePoolFrom("Loot.Modifiers.WeaponModifiers")]
	internal class WeaponModifierPool : ModifierPool
	{
		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsWeapon();
	}
}
