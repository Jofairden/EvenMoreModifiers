using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Ext;

namespace Loot.Pools
{
	[PopulatePoolFrom("Loot.Modifiers.EquipModifiers")]
	internal class AccessoryModifierPool : ModifierPool
	{
		public override bool CanRoll(ModifierContext ctx)
		{
			return ctx.Item.IsAccessory() || ctx.Item.IsArmor();
		}
	}
}
