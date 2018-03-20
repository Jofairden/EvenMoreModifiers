using Loot.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// Holds player-entity data and handles it
	/// </summary>
	public class EMMPlayer : ModPlayer
	{
		public static EMMPlayer PlayerInfo(Player player) => player.GetModPlayer<EMMPlayer>();

		// Globals for modifiers
		public bool holdingCursed;    // Whether currently holding a cursed item (take 1 damage per second)
		public int luck;              // Luck (TODO: Implement this)
		public float dodgeChance;     // Dodge chance (TODO: Implement this)

		// List of current debuff chances. Tuple format is [chance, buffType, buffTime]
		public List<Tuple<float, int, int>> debuffChances = new List<Tuple<float, int, int>>();

		public override void ResetEffects()
		{
			luck = 0;
			dodgeChance = 0;
			debuffChances.Clear();
		}

		public override void UpdateBadLifeRegen()
		{
			if (holdingCursed)
			{
				if (player.lifeRegen > 0)
				{
					player.lifeRegen = 0;
				}
				player.lifeRegen -= 2;
				player.lifeRegenTime = 0;
			}
			holdingCursed = false;
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			debuffChances.ForEach((x) => 
			{
				if (Main.rand.NextFloat() < x.Item1)
					target.AddBuff(x.Item2, x.Item3);
				Main.NewText($"{x.Item1} {x.Item2} {x.Item3}");
			});
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			debuffChances.ForEach((x) =>
			{
				if (Main.rand.NextFloat() < x.Item1)
					target.AddBuff(x.Item2, x.Item3);
			});
		}
	}
}
