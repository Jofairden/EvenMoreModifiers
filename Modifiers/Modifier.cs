using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier.
	/// </summary>
	public abstract class Modifier
	{
		// @todo: should we convert enums to their own object type?
		// a rarity has an associated color and required strength level

		/// <summary>
		/// Defines the name of a rarity
		/// </summary>
		public enum ModifierRarity
		{
			Common,
			Uncommon,
			Rare,
			Legendary,
			Transcendent
		}

		/// <summary>
		/// Defines the color of a rarity
		/// </summary>
		public enum RarityColor
		{
			White,
			Orange,
			Yellow,
			Red,
			Purple
		}

		public string Name { get; protected set; }

		/// <summary>
		/// The strength of our modifier. Our rarity is based on the modifier
		/// </summary>
		private float _strength;

		protected Modifier(string name)
		{
			Name = name;
			_strength = 0f;
		}
	}
}
