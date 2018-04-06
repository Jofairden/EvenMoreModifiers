using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CursedDamage : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"+{RoundedPower}% damage, but you are cursed", Color = Color.Lime}
			};

		public override float GetMinMagnitude(Item item) => 16f;
		public override float GetMaxMagnitude(Item item) => 30f;

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			damage = (int)Math.Ceiling(damage * (1 + RoundedPower / 100f));
		}

		public override void HoldItem(Item item, Player player)
		{
			base.HoldItem(item, player);
			ModifierPlayer.PlayerInfo(player).HoldingCursed = true;
		}
	}
}
