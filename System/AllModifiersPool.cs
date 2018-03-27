using System.Linq;

namespace Loot.System
{
	public sealed class AllModifiersPool : ModifierPool
	{
		public AllModifiersPool()
		{
			Effects = EMMLoader.Modifiers.Select(x => x.Value).ToArray();
		}
	}
}
