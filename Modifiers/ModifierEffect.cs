using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	public struct ModifierEffectTooltipLine
	{
		public string Text;
		public Color? Color;
	}

    /// <summary>
    /// Defines a modifier effect
    /// </summary>
    public abstract class ModifierEffect : ICloneable
	{
        public Mod Mod { get; internal set; }
        public uint Type { get; internal set; }
		public float Magnitude { get; internal set; } = 1f;
		public float Power { get; internal set; } = 1f;

		public virtual string Name => this.GetType().Name;
		public virtual float BasePower => 1f;
		public virtual float MinMagnitude => 1f;
		public virtual float MaxMagnitude => 1f;
		public virtual float RarityLevel => 1f;

		public abstract ModifierEffectTooltipLine[] TooltipLines { get; }

		internal float RollAndApplyMagnitude()
		{
			Magnitude = MinMagnitude + Main.rand.NextFloat() * (MaxMagnitude - MinMagnitude);
			Power = BasePower * Magnitude;
			return Power;
		}
		
		public virtual void UpdateItem(ModifierContext ctx, bool equipped = false)
		{

		}

		public virtual void ApplyItem(ModifierContext ctx)
        {

        }

        public virtual void ApplyNPC(ModifierContext ctx)
        {

        }

		public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{

		}

		public virtual void OnClone(ModifierEffect clone)
		{

		}

		public object Clone()
		{
			ModifierEffect clone = (ModifierEffect)this.MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			clone.Magnitude = Magnitude;
			clone.Power = Power;
			OnClone(clone);
			return clone;
		}
	}
}
