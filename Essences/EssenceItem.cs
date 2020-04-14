using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public virtual void PreRoll() // context + params
		{

		}

		public virtual void PostRoll() // context + params + result
		{

		}
	}
}
