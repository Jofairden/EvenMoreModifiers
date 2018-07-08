using Loot.Core;
using Microsoft.Xna.Framework;

namespace LootTests.TestRarities
{
	public class TestRarity : ModifierRarity
	{
		public override float RequiredRarityLevel => 1f;
		public override Microsoft.Xna.Framework.Color Color => Color.White;
	}
}