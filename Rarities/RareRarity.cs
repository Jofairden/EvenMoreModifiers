using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class RareRarity : ModifierRarity
	{
		public new static Func<TagCompound, RareRarity> DESERIALIZER = tag => (RareRarity)ModifierRarity.DESERIALIZER(tag);

		public override string Name => "Rare";
		public override Color Color => Color.Yellow;
		public override float RequiredRarityLevel => 2f;
	}
}
