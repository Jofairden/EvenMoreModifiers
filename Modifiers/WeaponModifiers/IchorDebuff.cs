using Loot.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.WeaponModifiers
{
	public sealed class IchorDebuff : WeaponDebuffModifier
	{
		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(maxMagnitude: 20f);
		}

		public override int BuffType => BuffID.Ichor;
		public override int BuffTime => 180;
	}
}
