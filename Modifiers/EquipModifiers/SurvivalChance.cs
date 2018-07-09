using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Modifiers.EquipModifiers
{
	public class SurvivalChance : EquipModifier
	{
		// TODO easier tooltip templating
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {
				Text =	$"+{Properties.RoundedPower}% chance to survive lethal blows" +
						$"{(Main.LocalPlayer.GetModPlayer<ModifierPlayer>().SurvivalChance >= ModifierPlayer.MAX_SURVIVAL_CHANCE ? $" (cap reached: {ModifierPlayer.MAX_SURVIVAL_CHANCE * 100f}%)" : "")}",
				Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 15f);
		}
		
		private bool _justModified;

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).SurvivalChance += Properties.RoundedPower / 100f;
			_justModified = true;
		}
		
		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			if (_justModified) ModifierPlayer.Player(player).SurvivalChance -= Properties.RoundedPower / 100f;
			_justModified = false;
		}
	}
}
