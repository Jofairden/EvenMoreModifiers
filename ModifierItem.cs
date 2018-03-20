using System;
using System.Collections.Generic;
using System.IO;
using Loot.Modifiers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot
{
	/// <summary>
	/// Holds entity based data
	/// </summary>
	public sealed class ModifierItem : GlobalItem
	{
		public static ModifierItem GetItemInfo(Item item) => item.GetGlobalItem<ModifierItem>();

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		// Special item parameters
		public float dontConsumeAmmo;      // % chance to not consume ammo
		public float dayDamageBonus;       // % damage bonus during the day
		public float nightDamageBonus;     // % damage bonus during the night
		public float missingHealthBonus;   // damage bonus scale from missing health
		public float velocityDamageBonus;  // damage bonus scale from velocity

		public override bool ConsumeAmmo(Item item, Player player)
		{
			// Ammo consumption chance
			return Main.rand.NextFloat() > dontConsumeAmmo;
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			if (Main.dayTime && dayDamageBonus > 0) damage = (int)Math.Ceiling(damage * (1 + dayDamageBonus / 100));
			if (!Main.dayTime && dayDamageBonus > 0) damage = (int)Math.Ceiling(damage * (1 + nightDamageBonus / 100));
			if (missingHealthBonus > 0)
			{
				// Formula ported from old mod
				float mag = (missingHealthBonus * ((player.statLifeMax2 - player.statLife) / (float)player.statLifeMax2) * 6);
				damage = (int)(damage * (1 + mag / 100));
			}
			if (velocityDamageBonus > 0 && player.velocity.Length() > 0)
			{
				// Formula ported from old mod
				float magnitude = velocityDamageBonus * player.velocity.Length() / 4;
				damage = (int)(damage * (1 + magnitude / 100));
			}
		}
	}

}
