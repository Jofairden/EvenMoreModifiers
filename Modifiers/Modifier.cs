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
    public enum ModifierContextMethod
    {
        OnReforge,
        OnCraft,
        OnPickup,
        UpdateItem,
        UpdateNPC
    }

    public struct ModifierContext
    {
        public ModifierContextMethod Method;
        public Player Player;
        public NPC NPC;
        public Item Item;
        public IDictionary<string, object> CustomData;
    }

	/// <summary>
	/// Defines a modifier.
	/// </summary>
	public abstract class Modifier : ICloneable
	{
        public uint Type { get; internal set; }
        public Mod Mod { get; internal set; }
        public ModifierRarity Rarity { get; internal set; }

        public float TotalRarityLevel => 
			Effects.Select(effect => effect.RarityLevel).DefaultIfEmpty(0).Sum();

        public IEnumerable<ModifierEffectTooltipLine[]> Description => 
            Effects.Select(effect => effect.TooltipLines);

        public virtual string Name => this.GetType().Name;
        public virtual float RollWeight => 1f;

        public ModifierEffect[] Effects { get; protected set; }

        internal ModifierRarity UpdateRarity()
        {
            Rarity = EMMLoader.GetModifierRarity(this);
            return Rarity;
        }

        internal bool _CanApply(ModifierContext ctx) {
            switch (ctx.Method)
            {
                case ModifierContextMethod.OnCraft:
                    return CanApplyCraft(ctx);
                case ModifierContextMethod.OnPickup:
                    return CanApplyPickup(ctx);
                case ModifierContextMethod.OnReforge:
                    return CanApplyReforge(ctx);
                case ModifierContextMethod.UpdateItem:
                    return CanUpdateItem(ctx);
                case ModifierContextMethod.UpdateNPC:
                    return CanUpdateItem(ctx);
                default:
                    return true;
            }
        }

        public virtual bool CanApplyCraft(ModifierContext ctx) => true;
        public virtual bool CanApplyPickup(ModifierContext ctx) => true;
        public virtual bool CanApplyReforge(ModifierContext ctx) => true;
        public virtual bool CanApplyItem(ModifierContext ctx) => true;
        public virtual bool CanUpdateItem(ModifierContext ctx) => true;
        public virtual bool CanUpdateNPC(ModifierContext ctx) => true;

        internal void ApplyItem(ModifierContext ctx)
        {
            if (_CanApply(ctx))
            {
                foreach (var effect in Effects)
                {
                    effect.ApplyItem(ctx);
                }
            }
        }

        internal void UpdateItem(ModifierContext ctx, bool equipped = false)
        {
            if (_CanApply(ctx))
            {
                foreach (var effect in Effects)
                {
                    effect.UpdateItem(ctx, equipped);
                }
            }
        }

        public override string ToString()
		    => LootUtils.JSLog(typeof(Modifier), this);

        public virtual void OnClone(Modifier clone)
        {

        }

        public object Clone()
        {
            Modifier clone = (Modifier)this.MemberwiseClone();
            clone.Type = Type;
            clone.Mod = Mod;
            clone.Rarity = (ModifierRarity)Rarity?.Clone();
            clone.Effects = 
                Effects?
                .Select(x => x?.Clone())
                .Cast<ModifierEffect>()
                .ToArray();
            OnClone(clone);
            return clone;
        }
    }
}
