using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Effects.AccessoryEffects
{
	public class InfernoEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = "Player has inferno", Color =  Color.IndianRed},
			};

		public override float RarityLevel => 3f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			if (equipped)
				ctx.Player.inferno = true;
		}
	}

	public class GodlyDefenseEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = "Player has godly defense", Color =  Color.SlateGray},
			};

		public override float RarityLevel => 5f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			if (equipped)
				ctx.Player.statDefense = 9999;
		}
	}

	public class MoreDamageEffect : ModifierEffect
	{
		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = "Player deals 100% more damage", Color =  Color.SlateGray},
			};

		public override float RarityLevel => 5f;

		public override void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			if (equipped)
			{
				ctx.Player.magicDamage *= 2f;
				ctx.Player.meleeDamage *= 2f;
				ctx.Player.rangedDamage *= 2f;
				ctx.Player.minionDamage *= 2f;
			}
		}
	}
}
