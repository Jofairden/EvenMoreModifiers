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
    public struct ModifierProperties
    {
        public bool CanApplyNPC;
        public bool CanApplyPlayer;
        public bool CanApplyItem;
        public ModifierItemProperties ItemProperties;

        public static ModifierProperties Default =>
            new ModifierProperties()
            {
                CanApplyNPC = false,
                CanApplyPlayer = false,
                CanApplyItem = false,
                ItemProperties =
                new ModifierItemProperties
                {
                    CanApplyReforge = false,
                    CanApplyCraft = false,
                    CanApplyPickup = false
                }
            };
    }

    public struct ModifierItemProperties
    {
        public bool CanApplyReforge;
        public bool CanApplyCraft;
        public bool CanApplyPickup;
    }

    public enum ModifierContextMethod
    {
        OnReforge,
        OnCraft,
        OnPickup,
        ApplyPlayer,
        ApplyNPC
    }

    public enum ModifierItemStatus
    {
        Inventory,
        Equipped
    }

    public struct ModifierContext
    {
        public ModifierContextMethod Method;
        public Player Player;
        public NPC NPC;
        public Item Item;
        public List<object> CustomData;
    }

	/// <summary>
	/// Defines a modifier.
	/// </summary>
	public abstract class Modifier
	{
		/// <summary>
		/// The strength of our modifier, which is the sum of our effects' strength.
		/// Our rarity is based on the total strength
		/// </summary>
		public float TotalStrength => 
			Effects.Select(effect => effect.Strength).DefaultIfEmpty(0).Sum();

        public IEnumerable<ModifierEffectTooltipLine[]> Description => 
            Effects.Select(effect => effect.TooltipLines);

        public virtual string Name => this.GetType().Name;
        public uint Type { get; internal set; }
        public Mod Mod { get; internal set; }

        /// <summary>
        /// The rarity of this modifier
        /// </summary>
        public ModifierRarity Rarity =>
            LootLoader.Rarities.Select(r => r.Value).OrderByDescending(r => r.RequiredStrength)
                .FirstOrDefault(r => r.MatchesRequirements(this));

        public virtual float Weight => 1f;

        /// <summary>
        /// The effects this modifier has
        /// </summary>
        public ModifierEffect[] Effects { get; protected set; }

        public ModifierProperties Properties { get; protected set; } = ModifierProperties.Default;

        internal bool _CanApply(ModifierContext ctx) {
            switch (ctx.Method)
            {
                case ModifierContextMethod.OnCraft:
                    return Properties.ItemProperties.CanApplyCraft && CanApplyCraft(ctx);
                case ModifierContextMethod.OnPickup:
                    return Properties.ItemProperties.CanApplyPickup && CanApplyPickup(ctx);
                case ModifierContextMethod.OnReforge:
                    return Properties.ItemProperties.CanApplyReforge && CanApplyReforge(ctx);
                case ModifierContextMethod.ApplyPlayer:
                    return Properties.CanApplyPlayer && CanApplyPlayer(ctx);
                default:
                    return true;
            }
        }

        public virtual bool CanApplyCraft(ModifierContext ctx) => true;
        public virtual bool CanApplyPickup(ModifierContext ctx) => true;
        public virtual bool CanApplyReforge(ModifierContext ctx) => true;
        public virtual bool CanApplyItem(ModifierContext ctx) => true;
  
        public virtual bool CanApplyNPC(ModifierContext ctx) => true;
        public virtual bool CanApplyPlayer(ModifierContext ctx) => true;

        internal void ApplyItem(ModifierContext ctx)
        {
            if (Properties.CanApplyItem
                && _CanApply(ctx))
            {
                foreach (var effect in Effects)
                {
                    effect.ApplyItem(ctx);
                }
            }
        }

        public override string ToString()
		{
			return LootUtils.JSLog(typeof(Modifier), this);
		}
	}
}
