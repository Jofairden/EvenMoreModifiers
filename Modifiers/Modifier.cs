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
		/// <summary>
		/// The name of our modifier, how it is identified
		/// </summary>
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
