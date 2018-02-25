using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Rarities
{
	public class LegendaryRarity : ModifierRarity
	{
		public new static Func<TagCompound, LegendaryRarity> DESERIALIZER = tag => (LegendaryRarity)ModifierRarity.DESERIALIZER(tag);

		public override string Name => "Legendary";
		public override Color Color => Color.Red;
		public override Color? OverrideNameColor => Color;
		public override string ItemPrefix => "Legendary";
		public override float RequiredRarityLevel => 4f;
	}
}
