using Loot.Api.Modifier;

namespace Loot.Effects
{
	/// <summary>
	/// Defines a "Null" effect which represents no effect safely
	/// Cannot be rolled normally
	/// </summary>
	public sealed class NullModifierEffect : ModifierEffect
	{
		public NullModifierEffect()
		{
			Mod = Loot.Instance;
			Type = 0;
		}
	}
}