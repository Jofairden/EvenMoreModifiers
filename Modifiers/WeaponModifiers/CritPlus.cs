using Loot.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CritPlus : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
			{
				new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% crit chance", Color = Color.Lime}
			};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 10f);
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			return base.CanRoll(ctx) && !ctx.Item.summon;
		}

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			crit = (int)Math.Min(100, crit + Properties.RoundedPower);
		}

		//public override ShaderEntity GetShaderEntity(Item item)
		//{
		//	return new ShaderEntity(item,
		//		GameShaders.Armor.GetShaderIdFromItemId(ItemID.MirageDye),
		//		drawLayer: ShaderDrawLayer.Front,
		//		drawOffsetStyle: ShaderDrawOffsetStyle.Alternate,
		//		shaderDrawColor: Color.IndianRed);
		//}

		//public override GlowmaskEntity GetGlowmaskEntity(Item item)
		//{
		//	return new GlowmaskEntity(item);
		//}
	}
}
