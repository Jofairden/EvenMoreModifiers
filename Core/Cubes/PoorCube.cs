using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public class PoorCube : MagicalCube
	{
		protected override string CubeName => "Poor Cube";
		protected override Color? OverrideNameColor => Color.White;
		protected override TooltipLine ExtraTooltip => new TooltipLine(mod, "PoorCube::Description::Add_Box",
			"Can only roll up 2 lines" +
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

		public override void SetRollLogic(ItemRollProperties properties)
		{
			base.SetRollLogic(properties);
			properties.MaxRollableLines = 2;
			properties.ForceModifierPool = mod.GetModifierPool<AllModifiersPool>();
		}
	}
}