using Loot.Api.Attributes;

namespace Loot.Api.Core
{
	/// <summary>
	/// Defines a "Null" modifier which represents a pool with no modifiers
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifierPool : ModifierPool
	{
		private static NullModifierPool _singleton;
		public static NullModifierPool INSTANCE
		{
			get => _singleton ?? (_singleton = new NullModifierPool());
			internal set => _singleton = value;
		}

		public override bool CanRoll(ModifierContext ctx) => false;

		public override float RollChance => 0f;

		private NullModifierPool()
		{
			ActiveModifiers = new Modifier[0];
		}
	}
}
