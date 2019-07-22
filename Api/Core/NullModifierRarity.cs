using Loot.Api.Attributes;
using Microsoft.Xna.Framework;

namespace Loot.Api.Core
{
	/// <summary>
	/// Defines a "Null" rarity which represents no rarity safely
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifierRarity : ModifierRarity
	{
		private static NullModifierRarity _singleton;
		public static NullModifierRarity INSTANCE
		{
			get => _singleton ?? (_singleton = new NullModifierRarity());
			internal set => _singleton = value;
		}

		public override Color Color => Color.White;

		private NullModifierRarity()
		{
		}
	}
}
