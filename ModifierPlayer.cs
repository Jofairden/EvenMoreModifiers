using System;
using System.Collections.Generic;
using System.Linq;
using Loot.System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// Holds player-entity data and handles it
	/// </summary>
	public class ModifierPlayer : ModPlayer
	{
		// Attempt rolling modifiers on first load
		public override void SetupStartInventory(IList<Item> items)
		{
			EMMWorld.WorldGenModifiersPass.GenerateModifiers(null, ModifierContextMethod.SetupStartInventory, items.Where(x => !x.IsAir), player);
		}

		public static ModifierPlayer PlayerInfo(Player player) => player.GetModPlayer<ModifierPlayer>();

		// Globals for modifiers
		public bool HoldingCursed;    // Whether currently holding a cursed item (take 1 damage per second)
		public int Luck;              // Luck (TODO: Implement this)
		public int BonusImmunityTime; // Extra immunity frames
		public int LightStrength;     // Light generation
		public int LifeRegen;         // Health regeneration
		public float DodgeChance;     // Dodge chance
		public float CritMulti = 1f; // Crit damage multiplier
		public float SurvivalChance;  // Chance to survive lethal blow
		public float ManaShield;      // % of damage redirected to mana
		public float PercentDefBoost; // % defense bonus
		public float HealthyFoesMulti = 1f; // Damage multiplier vs max life foes

		// List of current debuff chances. Tuple format is [chance, buffType, buffTime]
		// TODO with c#7 we should favor a named tuple (waiting for TML support)
		//public IList<(float chance, int type, int time)> DebuffChances;
		public IList<Tuple<float, int, int>> DebuffChances = new List<Tuple<float, int, int>>();

		public override void Initialize()
		{
			DebuffChances = new List<Tuple<float, int, int>>();
		}

		public override void ResetEffects()
		{
			Luck = 0;
			BonusImmunityTime = 0;
			LightStrength = 0;
			DodgeChance = 0;
			CritMulti = 1f;
			SurvivalChance = 0;
			ManaShield = 0;
			PercentDefBoost = 0;
			HealthyFoesMulti = 1f;
			DebuffChances.Clear();
		}

		public override void UpdateLifeRegen()
		{
			player.lifeRegen += LifeRegen / 30;
			LifeRegen %= 30;
		}

		public override void UpdateBadLifeRegen()
		{
			if (HoldingCursed)
			{
				if (player.lifeRegen > 0)
				{
					player.lifeRegen = 0;
				}
				player.lifeRegen -= 2;
				player.lifeRegenTime = 0;
			}
			HoldingCursed = false;
		}

		private void AttemptDebuff(NPC target)
		{
			//foreach (var debuff in DebuffChances)
			//{
			//	if (Main.rand.NextFloat() < debuff.chance)
			//		target.AddBuff(debuff.type, debuff.time);
			//}
			foreach (var x in DebuffChances)
			{
				if (Main.rand.NextFloat() < x.Item1)
					target.AddBuff(x.Item2, x.Item3);
			}
		}

		public override void PostUpdateEquips()
		{
			if (LightStrength > 0)
				Lighting.AddLight(player.Center, .15f * LightStrength, .15f * LightStrength, .15f * LightStrength);

			player.statDefense = (int)Math.Ceiling(player.statDefense * (1 + PercentDefBoost));
		}

		private void CritBonus(ref int damage, bool crit)
		{
			if (crit) damage = (int)Math.Ceiling(damage * CritMulti);
		}

		private void HealthyBonus(ref int damage, NPC npc)
		{
			if (npc.life == npc.lifeMax) damage = (int)(Math.Ceiling(damage * HealthyFoesMulti));
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Main.rand.NextFloat() < DodgeChance)
			{
				player.NinjaDodge();
				return false;
			}

			// If we have a mana shield (% damage redirected to mana)
			// Then try to redirect the damage
			int manaBlock = (int)Math.Ceiling(damage * ManaShield) * 2;
			if (manaBlock > 0 && player.statMana > 0)
			{
				// We cannot block more than how much mana we have
				if (manaBlock > player.statMana)
					manaBlock = player.statMana;

				damage -= manaBlock / 2;
				player.statMana -= manaBlock;
				player.manaRegenDelay = Math.Max(player.manaRegenDelay, 120);
			}

			return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
		}

		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			int frames = damage <= 1 ? BonusImmunityTime / 2 : BonusImmunityTime;
			if (player.immuneTime > 0) player.immuneTime += frames;
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			CritBonus(ref damage, crit);
			HealthyBonus(ref damage, target);
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			CritBonus(ref damage, crit);
			HealthyBonus(ref damage, target);
		}

		public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			CritBonus(ref damage, crit);
		}

		public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
		{
			CritBonus(ref damage, crit);
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			AttemptDebuff(target);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			AttemptDebuff(target);
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (Main.rand.NextFloat() < Math.Min(SurvivalChance, 0.8f))
			{
				player.statLife = 1;
				return false;
			}
			return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
		}
	}
}
