using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Modifiers;
using Microsoft.Xna.Framework;

namespace Loot.Rarities
{
	public class LegendaryRarity : ModifierRarity
	{
		public override string Name => "Legendary";
		public override Color Color => Color.Red;
		public override float RequiredStrength => 4f;
	}
}
