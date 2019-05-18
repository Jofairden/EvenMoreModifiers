using Microsoft.Xna.Framework;
using System;
using Loot.Api.Modifier;

namespace Loot.Rarities
{
	/// <summary>
	/// The rare rarity, before epic but after common
	/// </summary>
	public class RareRarity : ModifierRarity
	{
		public override string RarityName => "Rare";
		public override Color Color => Color.Yellow;
		public override float? UpgradeChance => 0.05f;
		public override float? DowngradeChance => 0.03f;
		public override Type Upgrade => typeof(EpicRarity);
		public override Type Downgrade => typeof(CommonRarity);
		public override float ExtraMagnitudePower => 0.1f;
	}
}
