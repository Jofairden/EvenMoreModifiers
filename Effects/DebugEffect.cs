using Loot.Modifiers;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace Loot.Effects
{
	public class MoreItemDamageEffect : ModifierEffect
	{
		public MoreItemDamageEffect()
		{

		}

		public override ModifierEffectTooltipLine[] Description => new[]
			{
				new ModifierEffectTooltipLine { Text = $"Deals {(int)Power}% more damage", Color = Color.Lime}
			};

		public override float MinMagnitude => 0.5f;
		public override float MaxMagnitude => 2.5f;
		public override float BasePower => 50f;

		public override float RarityLevel => 3f;

		public override void ApplyItem(ModifierContext ctx)
		{
			ctx.Item.damage = (int)Math.Ceiling((float)ctx.Item.damage * Power / 100f);
		}
	}

	public class InfernoEffect : ModifierEffect
	{
		public InfernoEffect()
		{

		}

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
		public GodlyDefenseEffect()
		{

		}

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
		public MoreDamageEffect()
		{

		}

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
