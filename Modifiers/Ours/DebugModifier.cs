using Loot.Effects;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.Ours
{
	internal class DebugModifier2 : Modifier
	{
		public DebugModifier2()
		{
			Effects = new[]
			{
				Loot.Instance.GetModifierEffect<MoreItemDamageEffect>()
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
			Effects = new ModifierEffect[]
			{
				Loot.Instance.GetModifierEffect<InfernoEffect>(),
				Loot.Instance.GetModifierEffect<GodlyDefenseEffect>(),
				Loot.Instance.GetModifierEffect<MoreDamageEffect>()
			};
		}

		public override bool CanApplyReforge(ModifierContext ctx)
		{
			return ctx.Item.accessory && Main.rand.NextBool(5);
		}

	}
}
