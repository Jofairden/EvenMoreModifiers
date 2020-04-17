using Loot.Api.Cubes;
using Loot.Api.Ext;
using Loot.Api.Strategy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Cubes
{
	class CopperCube : RerollingCube
	{
		public override int EssenceCraftCost => 10;
		protected override string CubeName => "Copper Cube";
		protected override Color? OverrideNameColor => Color.PeachPuff;

		protected override TooltipLine ExtraTooltip => new TooltipLine(mod, "BlackCube::Description::Add_Box",
			"Does nothing special for now")
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
			return RollingUtils.Strategies.Default;
		}
	}
}
