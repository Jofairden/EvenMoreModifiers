using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

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

		private bool _justModified;
		
		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).DodgeChance += Properties.RoundedPower / 100f;
			_justModified = true;
		}
		
		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			if(_justModified) ModifierPlayer.Player(player).DodgeChance -= Properties.RoundedPower / 100f;
			_justModified = false;
		}
	}
}
