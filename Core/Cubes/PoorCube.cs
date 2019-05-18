using Loot.Core.System.Modifier;
using Loot.Core.System.Strategy;
using Loot.Ext;
using Loot.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public class PoorCube : RerollingCube
	{
		protected override string CubeName => "Poor Cube";
		protected override Color? OverrideNameColor => Color.White;

		protected override TooltipLine ExtraTooltip => new TooltipLine(mod, "PoorCube::Description::Add_Box",
			"Maximum lines: 2" +
			"\nMaximum potential: Rare" +
			"\nAlways rolls from random modifiers")
		{
			overrideColor = OverrideNameColor
		};

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(copper: 30);
		}

		protected override void SafeStaticDefaults()
		{
		}

		public override IRollingStrategy<RollingStrategyContext> GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			var currentRarity = EMMItem.GetItemInfo(item).ModifierRarity;
			bool isLegendary = currentRarity?.GetType() == typeof(LegendaryRarity);
			bool isEpic = currentRarity?.GetType() == typeof(EpicRarity);
			bool forcedDowngrade = currentRarity != null && isLegendary || isEpic;
			if (forcedDowngrade)
			{
				properties.CanUpgradeRarity = ctx => false;
				properties.ForceModifierRarity = mod.GetModifierRarity<RareRarity>();
			}

			properties.MaxRollableLines = 2;
			properties.ForceModifierPool = mod.GetModifierPool<AllModifiersPool>();
			if (!forcedDowngrade)
			{
				properties.CanUpgradeRarity = ctx => ctx.Rarity.GetType() == typeof(CommonRarity);
			}
			return RollingUtils.Strategies.Normal;
		}
	}
}
