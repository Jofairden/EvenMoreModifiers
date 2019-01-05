using Loot.Core.Attributes;
using Loot.Core.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers.Defensive
{
	public class LifeRegenEffect : ModifierEffect
	{
		public int LifeRegen;

		public override void ResetEffects(ModifierPlayer player)
		{
			LifeRegen = 0;
		}

		[AutoDelegation("OnUpdateLifeRegen")]
		private void Regen(ModifierPlayer player)
		{
			player.player.lifeRegen += LifeRegen / 30;
			LifeRegen %= 30;
		}
	}

	[UsesEffect(typeof(LifeRegenEffect))]
	public class LifeRegen : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower} life regen/minute", Color =  Color.LimeGreen},
		};

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMaxMagnitude(15f)
				.WithRollChance(0.5f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).GetEffect<LifeRegenEffect>().LifeRegen += (int)Properties.RoundedPower;
		}
	}
}
