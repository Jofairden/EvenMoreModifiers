using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines the rarity of a modifier
	/// </summary>
	public abstract class ModifierRarity
	{
		public Mod Mod { get; internal set; }
		public ushort Type { get; internal set; }
		public abstract string Name { get; }
		public virtual float RequiredStrength { get; } = 0f;
		public virtual Color Color { get; } = Color.White;

		/// <summary>
		/// Returns if the specified modifier passes all the requirements
		/// By default, simply the required strength is checked
		/// </summary>
		public virtual bool MatchesRequirements(Modifier modifier)
		{
			return modifier.TotalStrength >= RequiredStrength;
		}

		public override string ToString()
		{
			return LootUtils.JSLog(typeof(ModifierRarity), this);
		}
	}
}
