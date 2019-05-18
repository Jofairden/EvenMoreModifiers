using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class AmmoReduce : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"{Properties.RoundedPower}% chance to not consume ammo");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			// Only apply on items that consume ammo
			return base.CanRoll(ctx) && ctx.Item.useAmmo > 0;
		}

		public override bool ConsumeAmmo(Item item, Player player)
		{
			return Main.rand.NextFloat() > Properties.RoundedPower / 100;
		}
	}
}
