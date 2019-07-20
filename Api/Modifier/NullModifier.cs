using Loot.Api.Attributes;

namespace Loot.Api.Modifier
{
	/// <summary>
	/// Defines a "Null" modifier which represents no modifier safely
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifier : Modifier
	{
		public override bool CanRoll(ModifierContext ctx) => false;
	}
}
