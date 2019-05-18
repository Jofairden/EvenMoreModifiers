using Loot.Api.Ext;
using Loot.Api.Modifier;
using Terraria;

namespace Loot.Modifiers.Base
{
	/// <summary>
	/// Defines a modifier that can roll on an equip item (armor or accessory)
	/// These modifiers will have 60% maximum Power on accessories
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class EquipModifier : Modifier
	{
		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return ModifierProperties.Builder
				.WithMagnitudeStrength(item.IsAccessory() ? .6f : 1f);
		}

		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsAccessory() || ctx.Item.IsArmor();
	}
}
