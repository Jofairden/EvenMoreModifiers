using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public class BlackCube : MagicalCube
	{
		protected override string CubeName => "Black Cube";
		protected override Color? OverrideNameColor => Color.LightSlateGray;
		protected override TooltipLine ExtraTooltip => new TooltipLine(mod, "BlackCube::Description::Add_Box",
			"Always rolls 4 lines" +
			"\nCan roll 25% stronger modifiers" +
			"\n+4 luck with this cube")
		{
			overrideColor = OverrideNameColor
		};

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(gold: 2);
		}

		protected override void SafeStaticDefaults()
		{
		}

		public override void SetRollLogic(ItemRollProperties properties)
		{
			base.SetRollLogic(properties);
			// set forced rolls to always roll 4 lines
			// set forced strength minimum 25%
			properties.MinModifierRolls = 4;
			properties.MagnitudePower = 1.25f;
			properties.ExtraLuck = 4;
		}
	}
}

