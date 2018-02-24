using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines the rarity of a modifier
	/// </summary>
	public abstract class ModifierRarity : ICloneable
	{
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }

		public virtual string Name => this.GetType().Name;
		public virtual Color? OverrideNameColor => null;
		public virtual string ItemPrefix => null;
		public virtual string ItemSuffix => null;

		public abstract float RequiredRarityLevel { get; }
		public abstract Color Color { get; }

		public RequirementPass.RequirementPassDelegate[] RequirementPasses { get; protected set; }
			= new[]
			{
				(RequirementPass.RequirementPassDelegate)((rarity, modifier)
					=> modifier.TotalRarityLevel >= rarity.RequiredRarityLevel)
			};

		public virtual bool MatchesRequirements(Modifier modifier)
			=> RequirementPasses.All(pass => pass.Invoke(this, modifier));

		public override string ToString()
			=> EMMUtils.JSLog(typeof(ModifierRarity), this);

		public virtual void OnClone(ModifierRarity clone)
		{

		}

		public object Clone()
		{
			ModifierRarity clone = (ModifierRarity)this.MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			clone.RequirementPasses =
				RequirementPasses
				.Select(x => x?.Clone())
				.Cast<RequirementPass.RequirementPassDelegate>()
				.ToArray();
			OnClone(clone);
			return clone;
		}
	}
}
