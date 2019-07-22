using Loot.Api.Core;
using Loot.Modifiers.Base;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class SpeedPlus : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% use speed");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithBasePower(1 / 5f)
				.WithMaxMagnitude(25f);
		}

		public override void Apply(Item item)
		{
			base.Apply(item);

			item.useTime = (int) (item.useTime * (1 - Properties.RoundedPower / 100f));
			item.useAnimation = (int) (item.useAnimation * (1 - Properties.RoundedPower / 100f));

			// Don't go below the minimum
			if (item.useTime < 2)
			{
				item.useTime = 2;
			}

			if (item.useAnimation < 2)
			{
				item.useAnimation = 2;
			}
		}

		/*public override float UseTimeMultiplier(Item item, Player player)
		{
			return 1 - Properties.RoundedPower / 100;
		}*/
	}
}
