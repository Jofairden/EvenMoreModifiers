using System;
using System.Collections.Generic;
using System.IO;
using Loot.Core.System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.WeaponModifiers
{
	public class DamagePlusDaytime : WeaponModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% damage during the {(_duringDay ? "day" : "night")}");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMinMagnitude(5f)
				.WithMaxMagnitude(15f)
				.WithRollChance(2f);
		}

		private bool _duringDay;

		public override void Roll(ModifierContext ctx, IEnumerable<Modifier> rolledModifiers)
		{
			base.Roll(ctx, rolledModifiers);
			_duringDay = Main.rand.NextBool();
		}

		// Here we showcase custom MP syncing

		public override void NetReceive(Item item, BinaryReader reader)
		{
			base.NetReceive(item, reader);
			_duringDay = reader.ReadBoolean();
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			base.NetSend(item, writer);
			writer.Write(_duringDay);
		}

		// Here we showcase custom loading and saving for a modifier

		public override void Load(Item item, TagCompound tag)
		{
			base.Load(item, tag);
			_duringDay = tag.GetBool("duringDay");
		}

		public override void Save(Item item, TagCompound tag)
		{
			base.Save(item, tag);
			tag.Add("duringDay", _duringDay);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			if (_duringDay && Main.dayTime || !_duringDay && !Main.dayTime)
			{
				damage = (int) Math.Ceiling(damage * (1 + Properties.RoundedPower / 100));
			}
		}
	}
}
