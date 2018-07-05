using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class PercentDefenseBonus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {Text = $"+{Properties.RoundedPower}% defense", Color = Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 8f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).PercentDefBoost += Properties.RoundedPower / 100f;
		}

		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			ModifierPlayer.Player(player).PercentDefBoost -= Properties.RoundedPower / 100f;
		}

		[AutoDelegation("OnPostUpdateEquips")]
		private void DefBoost(Player player)
		{
			player.statDefense = (int) Math.Ceiling(player.statDefense * (1 + ModifierPlayer.Player(player).PercentDefBoost));
		}
	}
}