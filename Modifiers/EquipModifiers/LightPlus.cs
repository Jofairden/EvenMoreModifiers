using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class LightPlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower} light", Color =  Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).LightStrength += (int)Properties.RoundedPower;
		}
		
		[AutoDelegation("OnResetEffects")]
		private void ResetEffects(Player player)
		{
			ModifierPlayer.Player(player).LightStrength -= (int)Properties.RoundedPower;
		}

		[AutoDelegation("OnPostUpdateEquips")]
		private void Light(Player player)
		{
			float str = ModifierPlayer.Player(player).LightStrength;
			if (str > 0)
				Lighting.AddLight(player.Center, .15f * str, .15f * str, .15f * str);
		}
	}
}
