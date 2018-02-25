using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class TranscendentRarity : ModifierRarity
	{
		public new static Func<TagCompound, TranscendentRarity> DESERIALIZER = tag => (TranscendentRarity)ModifierRarity.DESERIALIZER(tag);

		public override string Name => "Transcendent";
		public override Color Color => Color.Purple;
		public override float RequiredRarityLevel => 8f;
		public override string ItemSuffix => "of the Transcendent";
	}
}
