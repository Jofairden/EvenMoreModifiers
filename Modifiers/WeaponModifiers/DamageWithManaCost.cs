using System;
using System.Linq;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class DamageWithManaCost : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% damage, but added mana cost", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: 16f, maxMagnitude: 30f);
		}

		public override bool CanRoll(ModifierContext ctx)
			=> base.CanRoll(ctx) 
			   && ctx.Item.mana == 0;

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			damage = (int)Math.Ceiling(damage * (1 + Properties.RoundedPower / 100f));
		}

		public override bool CanUseItem(Item item, Player player)
		{
			return base.CanUseItem(item, player)
				   && player.statMana >= item.mana
				   && (item.useAmmo == 0 || player.inventory.Any(x => x.ammo == item.useAmmo));
		}

		public override void Apply(Item item)
		{
			base.Apply(item);
			item.mana = Math.Max((int)(25 * (item.useTime / 60.0)), 1);
		}
	}
}

