using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class TranscendentRarity : ModifierRarity
	{
		public override string Name => "Transcendent";
		public override Color Color => Color.Purple;
		public override float RequiredRarityLevel => 8f;
		public override string ItemSuffix => "of the Transcendent";
	}
}
