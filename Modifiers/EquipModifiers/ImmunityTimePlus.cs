using Loot.Core;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.EquipModifiers
{
	public class ImmunityEffect : ModifierEffect
	{
		public int BonusImmunityTime; // Extra immunity frames

		public override void ResetEffects(ModifierPlayer player)
		{
			BonusImmunityTime = 0;
		}

		[AutoDelegation("OnPostHurt")]
		[DelegationPrioritization(DelegationPrioritization.Late, 800)]
		private void Immunity(ModifierPlayer player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			int frames = damage <= 1
				? BonusImmunityTime / 2
				: BonusImmunityTime;
			if (player.player.immuneTime > 0)
			{
				player.player.immuneTime += frames;
			}
		}
	}

	[UsesEffect(typeof(ImmunityEffect))]
	public class ImmunityTimePlus : EquipModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine {Text = $"+{Properties.RoundedPower} immunity frames", Color = Color.LimeGreen},
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 20f);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			ModifierPlayer.Player(player).GetEffect<ImmunityEffect>().BonusImmunityTime += (int)Properties.RoundedPower;
		}
	}
}