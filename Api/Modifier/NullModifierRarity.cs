using Loot.Api.Attributes;
using Microsoft.Xna.Framework;

namespace Loot.Api.Modifier
{
	/// <summary>
	/// Defines a "Null" rarity which represents no rarity safely
	/// Cannot be rolled normally
	/// </summary>
	[DoNotLoad]
	public sealed class NullModifierRarity : ModifierRarity
	{
		public override Color Color => Color.White;
	}
}
