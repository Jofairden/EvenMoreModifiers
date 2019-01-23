using Loot.Core.System.Modifier;
using Microsoft.Xna.Framework;

namespace Loot.Rarities
{
	public class UncommonRarity : ModifierRarity
	{
		public override string RarityName => "Uncommon";
		public override Color Color => Color.Orange;
		public override float RequiredRarityLevel => 1f;
	}
}
