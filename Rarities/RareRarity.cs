using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class RareRarity : ModifierRarity
	{
		public override string Name => "Rare";
		public override Color Color => Color.Yellow;
		public override float RequiredRarityLevel => 2f;
	}
}
