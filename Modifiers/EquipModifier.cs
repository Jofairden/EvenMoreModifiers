using Loot.Core.System.Modifier;
using Loot.Ext;
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
		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return ModifierProperties.Builder
				.WithMagnitudeStrength(item.IsAccessory() ? .6f : 1f);
		}

		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsAccessory() || ctx.Item.IsArmor();
	}
}
