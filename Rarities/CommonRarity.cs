using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Modifiers;
using Microsoft.Xna.Framework;

namespace Loot.Rarities
{
	public class CommonRarity : ModifierRarity
	{
		public override string Name => "Common";
		public override Color Color => Color.White;
		public override float RequiredRarityLevel => 0f;
	}
}
