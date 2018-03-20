using Loot.Effects;
using Loot.Effects.AccessoryEffects;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.Ours
{
	internal class AccessoryModifier : Modifier
	{
		public AccessoryModifier()
		{
			Effects = new ModifierEffect[]
			{
				Loot.Instance.GetModifierEffect<InfernoEffect>(),
				Loot.Instance.GetModifierEffect<GodlyDefenseEffect>(),
				Loot.Instance.GetModifierEffect<MoreDamageEffect>()
			};
		}

		private bool AccessoryCheck(ModifierContext ctx) => ctx.Item.accessory && !ctx.Item.vanity;

		public override bool CanApplyReforge(ModifierContext ctx) => AccessoryCheck(ctx);
		public override bool CanApplyCraft(ModifierContext ctx) => AccessoryCheck(ctx);
		public override bool CanApplyPickup(ModifierContext ctx) => AccessoryCheck(ctx);

	}
}
