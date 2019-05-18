using Loot.Api.Modifier;
using Microsoft.Xna.Framework;

namespace Loot.Rarities
{
	/// <summary>
	/// Defines a "Null" rarity which represents no rarity safely
	/// Cannot be rolled normally
	/// </summary>
	public sealed class NullModifierRarity : ModifierRarity
	{
		public NullModifierRarity()
		{
			Mod = Loot.Instance;
			Type = 0;
		}

		public override Color Color => Color.White;
	}
}
