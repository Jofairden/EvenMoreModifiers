using Loot.Api.Attributes;

namespace Loot.Api.Modifier
{
	/// <summary>
	/// Defines a "Null" effect which represents no effect safely
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifierEffect : ModifierEffect
	{
	}
}
