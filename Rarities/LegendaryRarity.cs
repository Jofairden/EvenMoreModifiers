using Loot.Core.System.Modifier;
using Microsoft.Xna.Framework;
using System;

namespace Loot.Rarities
{
	/// <summary>
	/// The legendary rarity, before transcendent but after epic
	/// </summary>
	public class LegendaryRarity : ModifierRarity
	{
		public override string RarityName => "Legendary";
		public override Color Color => Color.LimeGreen;
		public override float? UpgradeChance => null;
		public override float? DowngradeChance => 0.5f;
		public override Type Upgrade => null;
		public override Type Downgrade => typeof(EpicRarity);
		public override float ExtraMagnitudePower => 0.5f;
		public override float ExtraLuck => 2;
	}
}
