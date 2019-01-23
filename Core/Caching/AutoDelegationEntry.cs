using Loot.Core.System.Modifier;
using Terraria;

namespace Loot.Core.Caching
{
	public class AutoDelegationEntry
	{
		public Item Item { get; set; }
		public Modifier Modifier { get; set; }

		public AutoDelegationEntry(Item item, Modifier modifier)
		{
			Item = item;
			Modifier = modifier;
		}
	}
}
