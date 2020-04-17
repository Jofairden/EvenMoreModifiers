using Loot.Api.Cubes;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Cubes
{
	public class BlackCube : RerollingCube
	{
		public override int EssenceCraftCost => 10;
		protected override string CubeName => "Black Cube";
		protected override Color? OverrideNameColor => Color.LightSlateGray;

		protected override TooltipLine ExtraTooltip => new TooltipLine(mod, "BlackCube::Description::Add_Box",
			"Always rolls 4 lines" +
			"\nMaximum potential: Legendary" +
			"\nCan roll 25% stronger modifiers" +
			"\n+4 luck with this cube")
		{
			overrideColor = OverrideNameColor
		};

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(copper: 1);
		}

		protected override void SafeStaticDefaults()
		{
		}

		public override RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.MinRollableLines = 4;
			properties.MagnitudePower = 1.25f;
			properties.ExtraLuck = 4;
			return RollingUtils.Strategies.Default;
		}
	}
}
