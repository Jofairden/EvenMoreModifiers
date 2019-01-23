using Loot.Core.System.Modifier;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class VelocityPlus : WeaponModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% projectile velocity");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return new ModifierPropertiesBuilder
			{
				DefaultBuilder = base.GetModifierProperties(item),
				MinMagnitude = 5f,
				MaxMagnitude = 10f,
				RoundPrecision = 1
			};
		}

		public override bool CanRoll(ModifierContext ctx)
			=> base.CanRoll(ctx) && ctx.Item.shoot > 0 && ctx.Item.shootSpeed > 0;

		public override void Apply(Item item)
		{
			base.Apply(item);
			// TODO needs better way
			item.shootSpeed *= Properties.RoundedPower / 100f + 1f;
		}
	}
}
