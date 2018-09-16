using System.Net.Mail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public class BlackCube : MagicalCube
	{
		protected override string CubeName => "Black Cube";
		protected override bool DisplayTier => true;

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(gold:2);
		}

		protected override void SafeStaticDefaults()
		{
		}

		public override void RightClick(Player player)
		{
			base.RightClick(player); // base
			
			// set forced rolls to always roll 4 lines
			// set forced strength minimum 25%
			
		}
	}
}

