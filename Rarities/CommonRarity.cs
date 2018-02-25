using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class CommonRarity : ModifierRarity
	{
		public override string Name => "Common";
		public override Color Color => Color.White;
		public override float RequiredRarityLevel => 0f;
	}
}
