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
	/// Defines a modifier that can inflict a debuff, applicable for weapons
	/// </summary>
	public abstract class WeaponDebuffModifier : WeaponModifier
	{
		public override ModifierTooltipLine[] TooltipLines => new[]
		{
			new ModifierTooltipLine { Text = $"+{Properties.RoundedPower}% chance to inflict {GetBuffName()} for {BuffTime/60f}s", Color = Color.Lime }
		};

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(minMagnitude: Math.Max(5, Math.Min(20, item.damage)), maxMagnitude: item.damage > 800 ? 100f : 50f);
		}

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
			//ModifierPlayer.PlayerInfo(player).DebuffChances.Add((chance: Power / 100, type: BuffType, time: BuffTime));
			ModifierPlayer.PlayerInfo(player).DebuffChances.Add(new Tuple<float, int, int>(Properties.RoundedPower / 100, BuffType, BuffTime));
		}
	}
}
