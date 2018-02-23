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

        public override ModifierEffectTooltipLine[] TooltipLines => new[]
        {
            new ModifierEffectTooltipLine { Text = $"Deals {(int)Magnitude * 100}% more damage", Color =  Color.Lime}
        };

        public override float Strength => 3f;

        public override void ApplyItem(ModifierContext ctx)
        {
            ctx.Item.damage *= (int)Magnitude;
        }
    }


    public class DebugEffect : ModifierEffect
	{
        public override ModifierEffectTooltipLine[] TooltipLines => new[]
        {
            new ModifierEffectTooltipLine { Text = "50% increased melee damage", Color =  Color.Lime},
            new ModifierEffectTooltipLine { Text = "Player has inferno", Color =  Color.IndianRed},
            new ModifierEffectTooltipLine { Text = "Player has godly defense", Color =  Color.SlateGray},
        };

		public override float Strength => 5f;

        public override void UpdatePlayer(EMMPlayer player, ModifierItemStatus status)
        {
            if (status == ModifierItemStatus.Equipped)
                player.player.GetModPlayer<LootDebugPlayer>().debugEffect = true;
        }
    }
}
