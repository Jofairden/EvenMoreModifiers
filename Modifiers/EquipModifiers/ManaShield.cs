using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Modifiers.EquipModifiers
{
	public class ManaShield : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% of damage taken is redirected to mana", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 6f);
		}

		private bool _justModified;
		
		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).ManaShield += Properties.RoundedPower / 100f;
			_justModified = true;
		}

		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			if (_justModified) ModifierPlayer.Player(player).ManaShield -= Properties.RoundedPower / 100f;
			_justModified = false;
		}
	
	}
}
