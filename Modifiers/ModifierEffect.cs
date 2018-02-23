using Microsoft.Xna.Framework;
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
    public abstract class ModifierEffect
    {
        public Mod Mod { get; internal set; }
        public uint Type { get; internal set; }
        public virtual string Name => this.GetType().Name;
        public abstract ModifierEffectTooltipLine[] TooltipLines { get; }
		public abstract float Strength { get; }
		public float Magnitude { get; internal set; } = 1f;
		public virtual float MinMagnitude => 1f;
		public virtual float MaxMagnitude => 1f;
		internal float RollMagnitude()
		{
			Magnitude = MinMagnitude + Main.rand.NextFloat() * (MaxMagnitude - MinMagnitude);
			return Magnitude;
		}
		
		public virtual void UpdatePlayer(EMMPlayer player, ModifierItemStatus status)
		{

		}
		//public virtual void UpdateNPC(EMMPlayer player, ModifierItemStatus modifierPlayerUpdate)
		//{

		//}
		public virtual void ApplyItem(ModifierContext ctx)
        {

        }
        public virtual void ApplyNPC(ModifierContext ctx)
        {

        }

		public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{

		}
		//public virtual void ApplyPlayer(ModifierContext ctx)
		//{

		//}
	}
}
