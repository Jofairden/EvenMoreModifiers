using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CursedDamage : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% damage, but you are cursed", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: 16f + 8f * (item.rare + 1), maxMagnitude: 30f + 20f * (item.rare + 1), uniqueRoll: true);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			damage = (int)Math.Ceiling(damage * (1 + Properties.RoundedPower / 100f));
		}

		public override void HoldItem(Item item, Player player)
		{
			base.HoldItem(item, player);
			ModifierPlayer.PlayerInfo(player).HoldingCursed = true;
		}
	}
}
