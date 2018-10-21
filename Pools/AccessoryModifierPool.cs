using Loot.Core;
using Loot.Core.Attributes;
using Loot.Ext;

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
