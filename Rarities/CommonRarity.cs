using Loot.Core;
using Loot.Core.System;
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
