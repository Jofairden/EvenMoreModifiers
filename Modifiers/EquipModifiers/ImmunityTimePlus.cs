using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class ImmunityTimePlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower} immunity frames", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 20f);
		}
		
		private bool _justModified;

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).BonusImmunityTime += (int)Properties.RoundedPower;
			_justModified = true;
		}
		
		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			if (_justModified) ModifierPlayer.Player(player).BonusImmunityTime -= (int)Properties.RoundedPower;
			_justModified = false;
		}

	}
}
