using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Modifiers
{
	/// <summary>
	/// A requirement pass has an inner function that checks modifier requirements
	/// </summary>
	public sealed class RequirementPass
	{
		/// <summary>
		/// Defines a requirement pass inner function
		/// </summary>
		public delegate bool RequirementPassDelegate(ModifierRarity rarity, Modifier modifier);
		public static RequirementPassDelegate MatchStrengthPass => new RequirementPassDelegate(((rarity, modifier) => modifier.Strength >= rarity.RequiredStrength));

		private readonly RequirementPassDelegate _innerFunc;

		private RequirementPass(RequirementPassDelegate func)
		{
			_innerFunc = func;
		}

		public bool Match(ModifierRarity rarity, Modifier modifier)
			=> _innerFunc(rarity, modifier);
	}
}
