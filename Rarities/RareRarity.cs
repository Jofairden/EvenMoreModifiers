using Loot.Core.System.Modifier;
using Microsoft.Xna.Framework;

namespace Loot.Rarities
{
	public class RareRarity : ModifierRarity
	{
		public override string RarityName => "Rare";
		public override Color Color => Color.Yellow;
		public override float RequiredRarityLevel => 2f;
	}
}
