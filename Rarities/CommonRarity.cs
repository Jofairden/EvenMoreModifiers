using Microsoft.Xna.Framework;
using System;
using Loot.Api.Modifier;

namespace Loot.Rarities
{
	/// <summary>
	/// Describes the common rarity, which every item starts with
	/// </summary>
	public class CommonRarity : ModifierRarity
	{
		public override string RarityName => "Common";
		public override Color Color => Color.White;
		public override float? UpgradeChance => 0.3f;
		public override Type Upgrade => typeof(RareRarity);
	}
}
