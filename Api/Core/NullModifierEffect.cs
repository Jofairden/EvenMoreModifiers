using Loot.Api.Attributes;

namespace Loot.Api.Core
{
	/// <summary>
	/// Defines a "Null" effect which represents no effect safely
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifierEffect : ModifierEffect
	{
		private static NullModifierEffect _singleton;
		public static NullModifierEffect INSTANCE
		{
			get => _singleton ?? (_singleton = new NullModifierEffect());
			internal set => _singleton = value;
		}

		private NullModifierEffect()
		{
		}
	}
}
