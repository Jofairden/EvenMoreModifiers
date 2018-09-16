using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class PercentDefBoostEffect : ModifierEffect
	{
		public float PercentDefBoost;

		public override void ResetEffects(ModifierPlayer player)
		{
			PercentDefBoost = 0f;
		}

		[AutoDelegation("OnPostUpdateEquips")]
		private void DefBoost(ModifierPlayer player)
		{
			player.player.statDefense = (int) Math.Ceiling(player.player.statDefense * (1 + PercentDefBoost));
		}
	}

	[UsesEffect(typeof(PercentDefBoostEffect))]
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
			ModifierPlayer.Player(player).GetEffect<PercentDefBoostEffect>().PercentDefBoost += Properties.RoundedPower / 100f;
		}
	}
}