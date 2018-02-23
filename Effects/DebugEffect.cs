using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Modifiers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Effects
{

    public class DebuggEffect2 : ModifierEffect
    {
        public override float MinMagnitude => 0.5f;
        public override float MaxMagnitude => 2.5f;
		public override float BasePower => 50f;

		public override ModifierEffectTooltipLine[] TooltipLines => new[]
        {
            new ModifierEffectTooltipLine { Text = $"Deals {(int)Power}% more damage", Color =  Color.Lime}
        };

        public override float RarityLevel => 3f;

        public override void ApplyItem(ModifierContext ctx)
        {
            ctx.Item.damage *= (int)(Power / 100f);
        }
    }


    public class DebugEffect : ModifierEffect
	{
        public override ModifierEffectTooltipLine[] TooltipLines => new[]
        {
            new ModifierEffectTooltipLine { Text = "50% increased melee damage",	Color =  Color.Lime},
            new ModifierEffectTooltipLine { Text = "Player has inferno",			Color =  Color.IndianRed},
            new ModifierEffectTooltipLine { Text = "Player has godly defense",		Color =  Color.SlateGray},
        };

		public override float RarityLevel => 5f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			if (equipped)
				(ctx.CustomData["EMMPlayer"] as EMMPlayer).player.GetModPlayer<LootDebugPlayer>().debugEffect = true;
		}
    }
}
