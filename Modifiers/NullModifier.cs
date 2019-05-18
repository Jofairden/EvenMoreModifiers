using Loot.Api.Modifier;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a "Null" modifier which represents no modifier safely
	/// Cannot be rolled normally
	/// </summary>
	public sealed class NullModifier : Modifier
	{
		public override bool CanRoll(ModifierContext ctx) => false;

		public NullModifier()
		{
			Mod = Loot.Instance;
			Type = 0;
		}
	}
}
