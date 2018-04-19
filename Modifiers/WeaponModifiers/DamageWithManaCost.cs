using System;
using System.Linq;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.WeaponModifiers
{
	public class DamageWithManaCost : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% damage, but adds +{_manaCost} mana cost", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: 15f + 5f * (item.rare + 1), maxMagnitude: 30f + 20f * (item.rare + 1));
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

		private int _manaCost;

		public override void Apply(Item item)
		{
			base.Apply(item);
			_manaCost = Math.Max((int)(25 * (item.useTime / 60.0)), 1);
			item.mana += _manaCost;
		}
	}
}

