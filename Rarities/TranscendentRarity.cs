using Microsoft.Xna.Framework;
using System;
using Loot.Api.Modifier;

namespace Loot.Rarities
{
	/// <summary>
	/// The Transcendent rarity, the most powerful rarity
	/// </summary>
	public class TranscendentRarity : ModifierRarity
	{
		public override string RarityName => "Transcendent";
		public override Color Color => Color.Purple;
		public override float? UpgradeChance => null;
		public override float? DowngradeChance => null;
		public override Type Upgrade => null;
		public override Type Downgrade => null;
		public override float ExtraMagnitudePower => 0.6f;
		public override float ExtraLuck => 4;
	}
}
