using Loot.Core.Attributes;
using Loot.Core.System.Modifier;
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
