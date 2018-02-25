using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class UncommonRarity : ModifierRarity
	{
		public new static Func<TagCompound, UncommonRarity> DESERIALIZER = tag => (UncommonRarity)ModifierRarity.DESERIALIZER(tag);

		public override string Name => "Uncommon";
		public override Color Color => Color.Orange;
		public override float RequiredRarityLevel => 1f;
	}
}
