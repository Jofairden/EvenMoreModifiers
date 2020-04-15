using Loot.Api.Ext;
using Loot.Api.Strategy;
using Terraria;

namespace Loot.Essences
{
	enum EssenceTier
	{
		I,
		II,
		III,
		IV,
		V
	}

	abstract class EssenceItem : TempItem
	{
		public abstract EssenceTier Tier { get; }

		private string GetTierText()
		{
			switch (Tier)
			{
				default:
				case EssenceTier.I:
					return "[c/ccd1d1:1]";
				case EssenceTier.II:
					return "[c/3498DB:2]";
				case EssenceTier.III:
					return "[c/58d68d:3]";
				case EssenceTier.IV:
					return "[c/eb984e:4]";
				case EssenceTier.V:
					return "[c/9b59b6:4]";
			}
		}

		public abstract string Description { get; }

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault($@"Essence
Tier: {GetTierText()}
{Description}
Used in an essence crafting device");
		}

		public virtual RollingStrategy GetRollingStrategy(Item item, RollingStrategyProperties properties)
		{
			properties.MaxRollableLines = 3;
			properties.MinModifierRolls = 1;
			if (Main.rand.NextBool(5))
				properties.MinModifierRolls = 2;
			if (Main.rand.NextBool(10))
				properties.ExtraLuck += 1;
			return RollingUtils.Strategies.Default;
		}
	}
}
