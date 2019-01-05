using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Loot.Core.System;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class DamageWithManaCost : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% damage, but adds +{_manaCost} mana cost", Color = Color.Lime}
			};

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMinMagnitude(5f)
				.WithMaxMagnitude(15f);
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			return base.CanRoll(ctx)
				   && ctx.Method != ModifierContextMethod.SetupStartInventory
				   && ctx.Item.mana == 0;
		}

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
			_manaCost = Math.Max((int)((float)item.useTime * (float)item.useTime / (float)GetMaxUseTime(item) / 10f), 1);
			item.mana += _manaCost;
		}

		private int GetMaxUseTime(Item item)
		{
			int number = 15;

			if (item.useTime <= 8)
			{
				return number;
			}

			while (number <= 55)
			{
				if (item.useTime <= number)
				{
					return number;
				}

				number += 5;
			}

			return 56;
		}
	}
}

