using System.Security.Cryptography;
using Loot;
using Loot.Core;
using LootTests.TestModifiers;

namespace LootTests.TestPools
{
	public class TestPool : ModifierPool
	{
		public override float RollChance => 1f;

		public TestPool()
		{
			Modifiers = new Modifier[]
			{
				EmptyModLoadShell.Instance.GetModifier<TestApplyMod>()
			};
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			return base.CanRoll(ctx);
		}
	}
}