using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class LightPlus : EquipModifier
	{
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower} light", Color =  Color.LimeGreen},
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 5f;

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.PlayerInfo(player).LightStrength += (int)RoundedPower;
		}
	}
}
