using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier effect
	/// </summary>
	public abstract class ModifierEffect
	{
		public Mod Mod { get; internal set; }
		public ushort Type { get; internal set; }
		public abstract string Description { get; }
		public abstract float Strength { get; }

		public abstract void Apply(object target, Item item, string methodName);
	}

	/// <summary>
	/// Defines a modifier.
	/// </summary>
	public sealed class Modifier
	{
		/// <summary>
		/// The strength of our modifier, which is the sum of our effects' strength.
		/// Our rarity is based on the total strength
		/// </summary>
		public float TotalStrength => 
			Effects.Select(effect => effect.Strength).DefaultIfEmpty(0).Sum();

		public string Description =>
			string.Join("\n", Effects.Select(effect => effect.Description).DefaultIfEmpty("No description"));

		/// <summary>
		/// The rarity of this modifier
		/// </summary>
		public ModifierRarity Rarity { get; }

		/// <summary>
		/// The effects this modifier has
		/// </summary>
		public List<ModifierEffect> Effects { get; }

		public Modifier(params ModifierEffect[] effects)
		{
			Effects = new List<ModifierEffect>(effects);
			//var p = Loader.Rarities.Select(r => r.Value).OrderByDescending(r => r.RequiredStrength);
			Rarity = Loader.Rarities.Select(r => r.Value).OrderByDescending(r => r.RequiredStrength)
				.FirstOrDefault(r => r.MatchesRequirements(this));
		}

		///// <summary>
		///// Attempts to apply a rarity and returns succession
		///// </summary>
		//public bool ApplyRarity(ModifierRarity rarity)
		//{
		//	if (rarity.MatchesRequirements(this))
		//	{
		//		Rarity = rarity;
		//		return true;
		//	}
		//	return false;
		//}

		public void Apply(object target, Item item, string methodName)
		{
			Effects.ForEach(effect => effect.Apply(target, item, methodName));
		}

		public override string ToString()
		{
			return LootUtils.JSLog(typeof(Modifier), this);
		}
	}
}
