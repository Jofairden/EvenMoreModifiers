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
		public uint Type { get; internal set; }
		public abstract string Name { get; }

        /// <summary>
        ///  The strength required that makes this rarity available
        /// </summary>
        public abstract float RequiredStrength { get; }

        /// <summary>
        /// The color this rarity will grant
        /// </summary>
        public abstract Color Color { get; }
		public virtual Color? OverrideNameColor => null;

		public virtual string ItemPrefix => null;
		public virtual string ItemSuffix => null;

		public RequirementPass.RequirementPassDelegate[] RequirementPasses { get; protected set; }
            = new[] { (RequirementPass.RequirementPassDelegate)((rarity, modifier) => modifier.TotalStrength >= rarity.RequiredStrength) };

        /// <summary>
        /// Returns if the specified modifier passes all the requirement passes
        /// By default, runs one pass that returns true if the total strength matches or surpasses the required strength
        /// </summary>
        public virtual bool MatchesRequirements(Modifier modifier)
		{
			return RequirementPasses.All(pass => pass.Invoke(this, modifier));
        }

		public override string ToString()
		{
			return LootUtils.JSLog(typeof(ModifierRarity), this);
		}
	}
}
