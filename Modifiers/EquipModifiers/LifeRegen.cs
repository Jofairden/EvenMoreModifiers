using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class LifeRegen : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower} life regen/minute", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 45f);
		}

		[AutoDelegation("OnUpdateLifeRegen")]
		private void LifeRegenHandler(Player player)
		{
			player.lifeRegen += ModifierPlayer.Player(player).LifeRegen / 30;
			ModifierPlayer.Player(player).LifeRegen %= 30;
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).LifeRegen += (int)Properties.RoundedPower;
		}
	}
}
