using System.Collections.Generic;
using Loot.Api.Attributes;

namespace Loot.Api.Core
{
	/// <summary>
	/// Defines a "Null" modifier which represents a pool with no modifiers
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifierPool : FiniteModifierPool
	{
		private static NullModifierPool _singleton;
		public static NullModifierPool INSTANCE
		{
			get => _singleton ?? (_singleton = new NullModifierPool());
			internal set => _singleton = value;
		}

		public override bool CanRoll(ModifierContext ctx) => false;

		public override float RollChance => 0f;

		public NullModifierPool(List<Modifier> modifiers = null) : base(modifiers)
		{
		}
	}
}
