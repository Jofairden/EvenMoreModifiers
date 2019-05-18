using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Modifiers.Base;
using Loot.Modifiers.EquipModifiers.Offensive;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Api.Delegators
{
	// TODO rework, make it like other delegators
	public class ModifierDelegatorProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public override GlobalProjectile Clone()
		{
			var clone = (ModifierDelegatorProjectile) MemberwiseClone();
			clone.SnapshotDebuffChances = new List<(int type, int time, float chance)>(SnapshotDebuffChances);
			return clone;
		}

		public ModifierDelegatorProjectile GetInfo(Projectile projectile, Mod givenMod = null)
			=> givenMod == null
				? projectile.GetGlobalProjectile<ModifierDelegatorProjectile>()
				: projectile.GetGlobalProjectile<ModifierDelegatorProjectile>(givenMod);

		public bool NeedsClear;
		public bool FirstTick;
		public float SnapshotHealthyFoesMulti = 1f;
		public float SnapshotCritMulti = 1f;
		public List<(int type, int time, float chance)> SnapshotDebuffChances = new List<(int type, int time, float chance)>();

		private void AttemptDebuff(Projectile projectile, Player target)
		{
			foreach (var x in SnapshotDebuffChances)
			{
				if (Main.rand.NextFloat() < x.chance)
				{
					target.AddBuff(x.type, x.time);
				}
			}
		}

		private void AttemptDebuff(Projectile projectile, NPC target)
		{
			foreach (var x in SnapshotDebuffChances)
			{
				if (Main.rand.NextFloat() < x.chance)
				{
					target.AddBuff(x.type, x.time);
				}
			}
		}

		private void HealthyBonus(Projectile projectile, ref int damage, NPC target)
		{
			if (target.life == target.lifeMax)
			{
				damage = (int) Math.Ceiling(damage * SnapshotHealthyFoesMulti);
			}
		}

		private void HealthyBonus(Projectile projectile, ref int damage, Player target)
		{
			if (target.statLife == target.statLifeMax2)
			{
				damage = (int) Math.Ceiling(damage * SnapshotHealthyFoesMulti);
			}
		}

		private void CritBonus(Projectile projectile, ref int damage, bool crit)
		{
			if (crit)
			{
				damage = (int) Math.Ceiling(damage * GetInfo(projectile).SnapshotCritMulti);
			}
		}

		public override bool PreAI(Projectile projectile)
		{
			var mproj = GetInfo(projectile);
			// On first tick, copy over player stats
			if (!mproj.FirstTick)
			{
				mproj.FirstTick = true;

				// Snapshot current player values
				if (projectile.owner != 255 && projectile.friendly && projectile.owner == Main.myPlayer)
				{
					var mplr = Main.LocalPlayer.GetModPlayer<ModifierDelegatorPlayer>();
					mproj.SnapshotDebuffChances = mplr.GetEffect<WeaponDebuffEffect>().DebuffChances.ToList();
					mproj.SnapshotHealthyFoesMulti = mplr.GetEffect<HealthyFoesEffect>().Multiplier;
					if (!projectile.minion) // minions do not crit
					{
						mproj.SnapshotCritMulti = mplr.GetEffect<CritDamagePlusEffect>().Multiplier;
					}
				}
				else if (projectile.owner == 255)
				{
					// TODO snapshot npc values. possible?
				}
			}

			if (NeedsClear)
			{
				mproj.SnapshotHealthyFoesMulti = 1f;
				mproj.SnapshotDebuffChances.Clear();
			}

			return base.PreAI(projectile);
		}

		// TODO I hate the copy pasta

		public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			HealthyBonus(projectile, ref damage, target);
			CritBonus(projectile, ref damage, crit);
			base.ModifyHitNPC(projectile, target, ref damage, ref knockback, ref crit, ref hitDirection);
		}

		// shouldn't run
		public override void ModifyHitPlayer(Projectile projectile, Player target, ref int damage, ref bool crit)
		{
			HealthyBonus(projectile, ref damage, target);
			CritBonus(projectile, ref damage, crit);
			base.ModifyHitPlayer(projectile, target, ref damage, ref crit);
		}

		public override void ModifyHitPvp(Projectile projectile, Player target, ref int damage, ref bool crit)
		{
			HealthyBonus(projectile, ref damage, target);
			CritBonus(projectile, ref damage, crit);
			base.ModifyHitPvp(projectile, target, ref damage, ref crit);
		}

		public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
			AttemptDebuff(projectile, target);
			base.OnHitNPC(projectile, target, damage, knockback, crit);
		}

		public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
		{
			AttemptDebuff(projectile, target);
			base.OnHitPlayer(projectile, target, damage, crit);
		}

		public override void OnHitPvp(Projectile projectile, Player target, int damage, bool crit)
		{
			AttemptDebuff(projectile, target);
			base.OnHitPvp(projectile, target, damage, crit);
		}
	}
}
