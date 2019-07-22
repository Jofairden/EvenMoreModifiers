using Loot.Api.Attributes;

namespace Loot.Api.Core
{
	/// <summary>
	/// Defines a "Null" modifier which represents no modifier safely
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifier : Modifier
	{
		private static NullModifier _singleton;
		public static NullModifier INSTANCE
		{
			get => _singleton ?? (_singleton = new NullModifier());
			internal set => _singleton = value;
		}

		public override bool CanRoll(ModifierContext ctx) => false;

		private NullModifier()
		{
		}
	}
}
