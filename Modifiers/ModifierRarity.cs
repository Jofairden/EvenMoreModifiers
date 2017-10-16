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
	public class ModifierRarity
	{
		//public static ModifierRarity Common = new ModifierRarity("Common", 0f, Color.White);
		//public static ModifierRarity Uncommon = new ModifierRarity("Uncommon", 1f, Color.Orange);
		//public static ModifierRarity Rare = new ModifierRarity("Rare", 2f, Color.Yellow);
		//public static ModifierRarity Legendary = new ModifierRarity("Legendary", 4f, Color.Red);
		//public static ModifierRarity Transcendent = new ModifierRarity("Transcendent", 8f, Color.Purple);

		public string Name { get; protected set; }
		public float RequiredStrength { get; protected set; }
		public Color Color { get; protected set; }

		private RequirementPass.RequirementPassDelegate[] RequirementPasses { get; }

		public ModifierRarity(string name, float requiredStrength, Color color, params RequirementPass.RequirementPassDelegate[] requirementPasses)
		{
			Name = name;
			RequiredStrength = requiredStrength;
			Color = color;
			RequirementPasses = requirementPasses ?? new [] {(RequirementPass.RequirementPassDelegate)((rarity, modifier) => true)};
		}

		/// <summary>
		/// Returns if the specified modifier passes all the requirements
		/// </summary>
		public bool MatchesRequirements(Modifier modifier)
			=> RequirementPasses.All(pass => pass.Invoke(this, modifier));

		public override string ToString()
		{
			return LootUtils.JSLog(typeof(ModifierRarity), this);
		}
	}
}
