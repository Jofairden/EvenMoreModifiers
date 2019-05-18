using Microsoft.Xna.Framework;
using System;
using Loot.Api.Modifier;

namespace Loot.Rarities
{
	/// <summary>
	/// The epic rarity, before legendary but after rare
	/// </summary>
	public class EpicRarity : ModifierRarity
	{
		public override string RarityName => "Epic";
		public override Color Color => Color.DeepSkyBlue;
		public override float? UpgradeChance => 0.03f;
		public override Type Upgrade => typeof(LegendaryRarity);
		public override Type Downgrade => typeof(RareRarity);
		public override float? DowngradeChance => 0.15f;
		public override float ExtraMagnitudePower => 0.25f;
	}
}
