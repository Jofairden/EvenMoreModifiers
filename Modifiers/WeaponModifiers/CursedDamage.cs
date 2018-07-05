using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CursedDamage : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% damage, but you are cursed while holding this item", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: 16f, maxMagnitude: 30f, uniqueRoll: true);
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			return base.CanRoll(ctx) && ctx.Method != ModifierContextMethod.SetupStartInventory;
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			damage = (int)Math.Ceiling(damage * (1 + Properties.RoundedPower / 100f));
		}

		public override void AttachDelegations(Item item, ModifierPlayer player)
		{
			player.OnUpdateBadLifeRegen += Curse;
		}

		public override void DetachDelegations(Item item, ModifierPlayer player)
		{
			player.OnUpdateBadLifeRegen -= Curse;
		}

		private void Curse(Player player)
		{
			if (!player.buffImmune[BuffID.Cursed])
			{
				if (player.lifeRegen > 0)
				{
					player.lifeRegen = 0;
				}
				player.lifeRegen -= 2;
				player.lifeRegenTime = 0;
			}
		}
	}
}
