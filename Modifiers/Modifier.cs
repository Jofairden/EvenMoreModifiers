using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Modifiers
{
	public delegate void ModifierApplyDelegate(object target);

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
		/// Describes our modifier (what does it do?)
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// The strength of our modifier. Our rarity is based on the modifier
		/// </summary>
		public float Strength { get; private set; }

		/// <summary>
		/// The rarity of this modifier
		/// </summary>
		public ModifierRarity Rarity { get; private set; }

		private readonly ModifierApplyDelegate _applyDelegate;

		protected Modifier(string name, string description = null, ModifierApplyDelegate applyDelegate = null)
		{
			Name = name;
			Description = description ?? string.Empty;
			Strength = 0f;
			Rarity = ModifierLoader.OurRarities.Common;
			_applyDelegate = applyDelegate ?? (ModifierApplyDelegate)((target) => { });
		}

		/// <summary>
		/// Explicitly state we want to modify the description
		/// </summary>
		/// <param name="newDescription"></param>
		public void ChangeDescription(string newDescription)
		{
			Description = newDescription;
		}

		/// <summary>
		/// Attempts to apply a rarity and returns succession
		/// </summary>
		public bool ApplyRarity(ModifierRarity rarity)
		{
			if (rarity.MatchesRequirements(this))
			{
				Rarity = rarity;
				return true;
			}
			return false;
		}

		public void Apply(object target)
			=> _applyDelegate.Invoke(target);

		public override string ToString()
		{
			return LootUtils.JSLog(typeof(Modifier), this);
		}
	}
}
