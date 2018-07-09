using System;
using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class CritDamagePlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {Text = $"+{Properties.RoundedPower}% crit multiplier", Color = Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 15f);
		}
		
		private bool _justModified;
		
		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).CritMultiplier += Properties.RoundedPower / 100f;
			_justModified = true;
		}
		
		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			var modplr = ModifierPlayer.Player(player);
			if (_justModified) modplr.CritMultiplier -= Properties.RoundedPower / 100f;
			if (modplr.CritMultiplier < 1f) modplr.CritMultiplier = 1f;
			_justModified = false;
		}
	}
}