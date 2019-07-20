using Loot.Api.Attributes;

namespace Loot.Api.Modifier
{
	/// <summary>
	/// Defines a "Null" modifier which represents a pool with no modifiers
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifierPool : ModifierPool
	{
		public override bool CanRoll(ModifierContext ctx) => false;

		public override float RollChance => 0f;

		public NullModifierPool()
		{
			ActiveModifiers = new Modifier[0];
		}
	}
}
