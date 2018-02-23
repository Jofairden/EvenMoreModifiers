using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Modifiers;
using Microsoft.Xna.Framework;

namespace Loot.Rarities
{
	public class TranscendentRarity : ModifierRarity
	{
		public override string Name => "Transcendent";
		public override Color Color => Color.Purple;
		public override float RequiredRarityLevel => 8f;
		public override string ItemSuffix => "of the Transcendent";
	}
}
