using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a debuff that can inflict a debuff, applicable for weapons
	/// </summary>
	public abstract class WeaponDebuffModifier : WeaponModifier
	{
		public override ModifierEffectTooltipLine[] Description => new[]
		{
			new ModifierEffectTooltipLine { Text = $"+{(int)Math.Round(Power)}% chance to inflict {GetBuffName()} for {BuffTime/60f}s", Color = Color.Lime }
		};

		public override float MinMagnitude => 0.02f;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 50f;

		public abstract int BuffType { get; }
		public abstract int BuffTime { get; }

		private string GetBuffName()
		{
			if (BuffType >= BuffID.Count)
				return BuffLoader.GetBuff(BuffType)?.DisplayName.GetTranslation(LanguageManager.Instance.ActiveCulture) ?? "null";

			return Lang.GetBuffName(BuffType);
		}

		public override void HoldItem(Item item, Player player)
		{
			ModifierPlayer.PlayerInfo(player).debuffChances.Add(new Tuple<float, int, int>(Power / 100, BuffType, BuffTime));
		}
	}
}
