namespace Loot.Core.System.Modifier
{
	/// <summary>
	/// Defines a "Null" modifier which represents a pool with no modifiers
	/// Cannot be rolled normally
	/// </summary>
	public sealed class NullModifierPool : ModifierPool
	{
		public override bool CanRoll(ModifierContext ctx) => false;

		public override float RollChance => 0f;

		public NullModifierPool()
		{
			Mod = Loot.Instance;
			Type = 0;
			ActiveModifiers = new Modifier[0];
		}
	}
}
