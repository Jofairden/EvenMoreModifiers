using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class DodgeChance : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% dodge chance", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 5f);
		}


		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.PlayerInfo(player).DodgeChance += Properties.RoundedPower / 100;
		}
	}
}
