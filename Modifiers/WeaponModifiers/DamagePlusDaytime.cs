using System;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.WeaponModifiers
{
	public class DamagePlusDaytime : WeaponModifier
	{
		public override ModifierTooltipLine[] Description => new[]
			{
				new ModifierTooltipLine { Text = $"+{RoundedPower}% damage during the {(_duringDay ? "day" : "night")}", Color = Color.Lime}
			};

		public override float MinMagnitude => 1.0f / 15;
		public override float MaxMagnitude => 1.0f;
		public override float BasePower => 15f;
		public override float RollChance => base.RollChance * 2f; // because this is essentially 2 mods in one, double the roll chance

		private bool _duringDay;

		public override void Apply(Item item)
		{
			base.Apply(item);
			_duringDay = Main.rand.NextBool();
		}

		// Here we showcase custom loading and saving for a modifier

		public override void Load(TagCompound tag)
		{
			base.Load(tag);
			_duringDay = tag.GetBool("duringDay");
		}

		public override void Save(TagCompound tag)
		{
			base.Save(tag);
			tag.Add("duringDay", _duringDay);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			if (_duringDay && Main.dayTime || !_duringDay && !Main.dayTime)
				damage = (int) Math.Ceiling(damage * (1 + Power / 100));
		}
	}
}
