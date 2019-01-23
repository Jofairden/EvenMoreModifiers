using Loot.Core.System.Modifier;
using Microsoft.Xna.Framework;

namespace Loot.Rarities
{
	public class CommonRarity : ModifierRarity
	{
		public override string RarityName => "Common";
		public override Color Color => Color.White;
		public override float RequiredRarityLevel => 0f;
	}
}
