using Loot.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Loot.Modifiers.Ours
{
    internal class DebugModifier2 : Modifier
    {
        public DebugModifier2()
        {
            Properties = new ModifierProperties()
            {
                CanApplyPlayer = true,
                ItemProperties = new ModifierItemProperties
                {
                    CanApplyReforge = true
                },
            };

            Effects = new[]
            {
                Loot.Instance.GetModifierEffect<DebuggEffect2>()
            };
        }

        public override bool CanApplyReforge(ModifierContext ctx)
        {
            return ctx.Item.damage > 0 && Main.rand.NextBool(5);
        }
    }

    internal class DebugModifier : Modifier
    {
        public DebugModifier()
        {
            Properties = new ModifierProperties()
            {
                CanApplyPlayer = true,
                ItemProperties = new ModifierItemProperties
                {
                    CanApplyReforge = true
                },
            };

            Effects = new[]
            {
                Loot.Instance.GetModifierEffect<DebugEffect>()
            };
        }

        public override bool CanApplyReforge(ModifierContext ctx)
        {
            return ctx.Item.accessory && Main.rand.NextBool(5);
        }
    }
}
