using Loot.Api.Modifier;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class KnockbackPlus : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% knockback");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(10f);
		}

		public override bool CanRoll(ModifierContext ctx)
			=> base.CanRoll(ctx) && ctx.Item.knockBack > 0;

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			base.GetWeaponKnockback(item, player, ref knockback);
			knockback *= Properties.RoundedPower / 100f + 1;
		}
	}
}
