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
		public override ModifierTooltipLine[] Description => new[]
		{
			new ModifierTooltipLine { Text = $"+{RoundedPower}% chance to inflict {GetBuffName()} for {BuffTime/60f}s", Color = Color.Lime }
		};

		public override float GetMinMagnitude(Item item) => 1f;
		public override float GetMaxMagnitude(Item item) => 50f;

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
			ModifierPlayer.PlayerInfo(player).DebuffChances.Add(new Tuple<float, int, int>(RoundedPower / 100, BuffType, BuffTime));
		}
	}
}
