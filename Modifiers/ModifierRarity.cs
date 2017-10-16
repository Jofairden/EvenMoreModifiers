using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines the rarity of a modifier
	/// </summary>
	public abstract class ModifierRarity
	{
		public string Name { get; protected set; }
		public float RequiredStrength { get; protected set; }
		public Color Color { get; protected set; }

		protected ModifierRarity(string name, float requiredStrength, Color color)
		{
			Name = name;
			RequiredStrength = requiredStrength;
			Color = color;
		}
	}
}
