using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class DamageWithManaCost : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"+{RoundedPower}% damage, but added mana cost", Color = Color.Lime}
			};

		public override float GetMinMagnitude(Item item) => 16f;
		public override float GetMaxMagnitude(Item item) => 30f;

		public override bool CanRoll(ModifierContext ctx) 
			=> base.CanRoll(ctx) && ctx.Item.mana == 0;

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			damage = (int)Math.Ceiling(damage * (1 + RoundedPower / 100f));
		}

		public override void Apply(Item item)
		{
			base.Apply(item);
			item.mana = Math.Max((int)(25 * (item.useTime / 60.0)), 1);
		}
	}
}

