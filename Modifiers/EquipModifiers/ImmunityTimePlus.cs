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

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).BonusImmunityTime += (int)Properties.RoundedPower;
		}
		
		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			ModifierPlayer.Player(player).BonusImmunityTime -= (int)Properties.RoundedPower;
		}

		[AutoDelegation("OnPostHurt")]
		private void Immunity(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			int frames = damage <= 1 
				? ModifierPlayer.Player(player).BonusImmunityTime / 2 
				: ModifierPlayer.Player(player).BonusImmunityTime;
			if (player.immuneTime > 0) player.immuneTime += frames;
		}
	}
}
