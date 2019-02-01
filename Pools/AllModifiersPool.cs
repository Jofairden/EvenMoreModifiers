using Loot.Core.System.Loaders;
using Loot.Core.System.Modifier;
using System.Collections.Generic;
using System.Linq;

namespace Loot.Pools
{
	/// <summary>
	/// A modifier pool that always consists of all modifiers
	/// </summary>
	public sealed class AllModifiersPool : ModifierPool
	{
		public override float RollChance => 0f;

		public override IEnumerable<Modifier> GetModifiers()
			=> ContentLoader.Modifier.Content.Select(x => x.Value).ToArray();
	}
}
