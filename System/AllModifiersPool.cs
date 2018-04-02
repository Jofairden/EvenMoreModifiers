using System.Linq;

namespace Loot.System
{
	public sealed class AllModifiersPool : ModifierPool
	{
		public override float RollChance => 0f;

		public AllModifiersPool()
		{
			Modifiers = EMMLoader.Modifiers.Select(x => x.Value).ToArray();
		}
	}
}
