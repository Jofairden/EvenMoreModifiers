using Loot.Core;
using Terraria;

namespace LootTests.TestModifiers
{
	public class TestApplyMod : Modifier
	{
		public override void Apply(Item item)
		{
			item.damage = 9001;
		}
	}
}