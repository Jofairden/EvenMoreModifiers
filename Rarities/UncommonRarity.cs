using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class UncommonRarity : ModifierRarity
	{
		public override string Name => "Uncommon";
		public override Color Color => Color.Orange;
		public override float RequiredRarityLevel => 1f;
	}
}
