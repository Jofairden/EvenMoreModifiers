using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Loaders;

namespace Loot.Pools
{
	/// <summary>
	/// A modifier pool that always consists of all modifiers
	/// Cannot be rolled normally
	/// </summary>
	public sealed class AllModifiersPool : ModifierPool
	{
		public override float RollChance => 0f;

		public override IEnumerable<Modifier> GetModifiers()
			=> ContentLoader.Modifier.Content.Select(x => x.Value).ToArray();

		public AllModifiersPool()
		{
			Mod = Loot.Instance;
			Type = 1;
		}
	}
}
