using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class PercentDefenseBonus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower}% defense", Color =  Color.LimeGreen},
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 8f;

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.PlayerInfo(player).PercentDefBoost += RoundedPower / 100;
		}
	}
}
