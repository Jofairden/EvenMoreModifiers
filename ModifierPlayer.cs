using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Core;
using Loot.Modifiers;
using Loot.Modifiers.WeaponModifiers;
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
			EMMWorld.WorldGenModifiersPass.GenerateModifiers(null, ModifierContextMethod.SetupStartInventory, items.Where(x => !x.IsAir && x.IsModifierRollableItem()), player);
		}
		
		public override void OnEnterWorld(Player player)
		{
			EMMWorld.WorldGenModifiersPass.GenerateModifiers(null, ModifierContextMethod.FirstLoad, player.inventory.Where(x => !x.IsAir && x.IsModifierRollableItem()).Concat(player.armor.Where(x => !x.IsAir && !x.IsModifierRollableItem())), player);
		}

		public static ModifierPlayer Player(Player player) => player.GetModPlayer<ModifierPlayer>();

		// Globals for modifiers
		public int Luck;              		// Luck (TODO: Implement this)
		public int BonusImmunityTime; 		// Extra immunity frames
		public int LightStrength;     		// Light generation
		public int LifeRegen;         		// Health regeneration
		public float DodgeChance;     		// Dodge chance
		public float CritMultiplier = 1f; 		// Crit damage multiplier
		public float SurvivalChance;  		// Chance to survive lethal blow
		public float ManaShield;      		// % of damage redirected to mana
		public float PercentDefBoost; 		// % defense bonus
		public float HealthyFoesMulti = 1f; // Damage multiplier vs max life foes

		public static readonly float MAX_SURVIVAL_CHANCE = 0.5f;

		// List of current debuff chances. Tuple format is [chance, buffType, buffTime]
		// TODO with c#7 we should favor a named tuple (waiting for TML support)
		//public IList<(float chance, int type, int time)> DebuffChances;
		public IList<RandomDebuff.DebuffTrigger> DebuffChances = new List<RandomDebuff.DebuffTrigger>();

		public delegate void VoidEventRaiser(Player player);
		
		public event VoidEventRaiser OnInitialize;
		public override void Initialize()
		{
			DebuffChances = new List<RandomDebuff.DebuffTrigger>();
			OnInitialize?.Invoke(player);
		}

		public event VoidEventRaiser OnPreUpdate;
		public override void PreUpdate()
		{
			OnPreUpdate?.Invoke(player);
		}

		public event VoidEventRaiser OnResetEffects;
		public override void ResetEffects()
		{
			DebuffChances.Clear();

			if (player.GetModPlayer<ModifierCachePlayer>().Ready)
			{
				OnResetEffects?.Invoke(player);
			}
		}

		public event VoidEventRaiser OnUpdateLifeRegen;
		public override void UpdateLifeRegen()
		{
			OnUpdateLifeRegen?.Invoke(player);

			LifeRegenHandler(player);
		}
		
		private void LifeRegenHandler(Player player)
		{
			player.lifeRegen += LifeRegen / 30;
			Player(player).LifeRegen %= 30;
		}

		public event VoidEventRaiser OnUpdateBadLifeRegen;
		public override void UpdateBadLifeRegen()
		{
			OnUpdateBadLifeRegen?.Invoke(player);
		}

		public event VoidEventRaiser OnPostUpdateEquips;
		public override void PostUpdateEquips()
		{
			OnPostUpdateEquips?.Invoke(player);
			
			Light(player);
		}
		
		private void Light(Player player)
		{
			float str = LightStrength;
			if (str > 0) Lighting.AddLight(player.Center, .15f * str, .15f * str, .15f * str);
		}

		public delegate bool PreHurtEventRaiser(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
		public event PreHurtEventRaiser OnPreHurt;	
		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			bool b=  base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
			if (OnPreHurt != null)
				b &= OnPreHurt.Invoke(player, pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);

			b &= TryDodge();
			if (b) b &= ManaBlock(player, ref damage);
			return b;
		}
		
		private bool TryDodge()
		{
			if (Main.rand.NextFloat() < DodgeChance)
			{
				player.NinjaDodge();
				return false;
			}

			return true;
		}
		
		private bool ManaBlock(Player player, ref int damage)
		{
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

			return true;
		}

		public delegate void PostHurtEventRaiser(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit);
		public event PostHurtEventRaiser OnPostHurt;	
		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			OnPostHurt?.Invoke(player, pvp, quiet, damage, hitDirection, crit);
			
			Immunity(player, damage);
		}
				
		private void Immunity(Player player, double damage)
		{
			int frames = damage <= 1 
				? BonusImmunityTime / 2 
				: BonusImmunityTime;
			if (player.immuneTime > 0) player.immuneTime += frames;
		}

		public delegate void ModifyHitNPCEventRaiser(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit);
		public event ModifyHitNPCEventRaiser OnModifyHitNPC;
		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			OnModifyHitNPC?.Invoke(player, item, target, ref damage, ref knockback, ref crit);
			
			if (target.life == target.lifeMax) HealthyFoes(ref damage);
			CritBonus(player, ref damage, crit);
		}

		public delegate void ModifyHitPvpEventRaiser(Player player, Item item, Player target, ref int damage, ref bool crit);
		public event ModifyHitPvpEventRaiser OnModifyHitPvp;
		public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			OnModifyHitPvp?.Invoke(player, item, target, ref damage, ref crit);
			
			if (target.statLife == target.statLifeMax2) HealthyFoes(ref damage);
			CritBonus(player, ref damage, crit);
		}

		private void HealthyFoes(ref int damage)
		{
			damage = (int) (Math.Ceiling(damage * HealthyFoesMulti));
		}
		
		private void CritBonus(Player player, ref int damage, bool crit)
		{
			if (crit) damage = (int) Math.Ceiling(damage * CritMultiplier);
		}

		public delegate void OnHitNPCEventRaiser(Player player, Item item, NPC target, int damage, float knockback, bool crit);
		public event OnHitNPCEventRaiser OnOnHitNPC;
		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			OnOnHitNPC?.Invoke(player, item, target, damage, knockback, crit);
		}

		public delegate void OnHitPvpEventRaiser(Player player, Item item, Player target, int damage, bool crit);
		public event OnHitPvpEventRaiser OnOnHitPvp;	
		public override void OnHitPvp(Item item, Player target, int damage, bool crit)
		{
			OnOnHitPvp?.Invoke(player, item, target, damage, crit);
		}

		public delegate bool PreKillEventRaiser(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
		public event PreKillEventRaiser OnPreKill;
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{			
			bool b = base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
			if (OnPreKill != null)
				b &= OnPreKill.Invoke(player, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);

			b &= SurviveEvent(player);
			return b;
		}
		
		private bool SurviveEvent(Player player)
		{
			if (Main.rand.NextFloat() < Math.Min(SurvivalChance, MAX_SURVIVAL_CHANCE))
			{
				player.statLife = 1;
				return false;
			}

			return true;
		}
		
		
	}
}
