using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Loot
{
	// TODO If we want ModPlayer code confined to Modifier classes, we will need to make a way to access it in Modifier
	// We can do it similar to ModifierItem

	//public class ModifierPlayer2 : ModPlayer
	//{
	//	public override bool CloneNewInstances => base.CloneNewInstances;

	//	public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
	//	{
	//		base.AnglerQuestReward(rareMultiplier, rewardItems);
	//	}

	//	public override bool Autoload(ref string name)
	//	{
	//		return base.Autoload(ref name);
	//	}

	//	public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
	//	{
	//		return base.CanBeHitByNPC(npc, ref cooldownSlot);
	//	}

	//	public override bool CanBeHitByProjectile(Projectile proj)
	//	{
	//		return base.CanBeHitByProjectile(proj);
	//	}

	//	public override bool? CanHitNPC(Item item, NPC target)
	//	{
	//		return base.CanHitNPC(item, target);
	//	}

	//	public override bool? CanHitNPCWithProj(Projectile proj, NPC target)
	//	{
	//		return base.CanHitNPCWithProj(proj, target);
	//	}

	//	public override bool CanHitPvp(Item item, Player target)
	//	{
	//		return base.CanHitPvp(item, target);
	//	}

	//	public override bool CanHitPvpWithProj(Projectile proj, Player target)
	//	{
	//		return base.CanHitPvpWithProj(proj, target);
	//	}

	//	public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
	//	{
	//		base.CatchFish(fishingRod, bait, power, liquidType, poolSize, worldLayer, questFish, ref caughtType, ref junk);
	//	}

	//	public override void clientClone(ModPlayer clientClone)
	//	{
	//		base.clientClone(clientClone);
	//	}

	//	public override bool ConsumeAmmo(Item weapon, Item ammo)
	//	{
	//		return base.ConsumeAmmo(weapon, ammo);
	//	}

	//	public override void CopyCustomBiomesTo(Player other)
	//	{
	//		base.CopyCustomBiomesTo(other);
	//	}

	//	public override bool CustomBiomesMatch(Player other)
	//	{
	//		return base.CustomBiomesMatch(other);
	//	}

	//	public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
	//	{
	//		base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
	//	}

	//	public override bool Equals(object obj)
	//	{
	//		return base.Equals(obj);
	//	}

	//	public override void FrameEffects()
	//	{
	//		base.FrameEffects();
	//	}

	//	public override void GetDyeTraderReward(List<int> rewardPool)
	//	{
	//		base.GetDyeTraderReward(rewardPool);
	//	}

	//	public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
	//	{
	//		base.GetFishingLevel(fishingRod, bait, ref fishingLevel);
	//	}

	//	public override int GetHashCode()
	//	{
	//		return base.GetHashCode();
	//	}

	//	public override Texture2D GetMapBackgroundImage()
	//	{
	//		return base.GetMapBackgroundImage();
	//	}

	//	public override void GetWeaponCrit(Item item, ref int crit)
	//	{
	//		base.GetWeaponCrit(item, ref crit);
	//	}

	//	public override void GetWeaponDamage(Item item, ref int damage)
	//	{
	//		base.GetWeaponDamage(item, ref damage);
	//	}

	//	public override void GetWeaponKnockback(Item item, ref float knockback)
	//	{
	//		base.GetWeaponKnockback(item, ref knockback);
	//	}

	//	public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
	//	{
	//		base.Hurt(pvp, quiet, damage, hitDirection, crit);
	//	}

	//	public override void Initialize()
	//	{
	//		base.Initialize();
	//	}

	//	public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
	//	{
	//		base.Kill(damage, hitDirection, pvp, damageSource);
	//	}

	//	public override void Load(TagCompound tag)
	//	{
	//		base.Load(tag);
	//	}

	//	public override void LoadLegacy(BinaryReader reader)
	//	{
	//		base.LoadLegacy(reader);
	//	}

	//	public override void MeleeEffects(Item item, Rectangle hitbox)
	//	{
	//		base.MeleeEffects(item, hitbox);
	//	}

	//	public override float MeleeSpeedMultiplier(Item item)
	//	{
	//		return base.MeleeSpeedMultiplier(item);
	//	}

	//	public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
	//	{
	//		base.ModifyDrawHeadLayers(layers);
	//	}

	//	public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
	//	{
	//		base.ModifyDrawInfo(ref drawInfo);
	//	}

	//	public override void ModifyDrawLayers(List<PlayerLayer> layers)
	//	{
	//		base.ModifyDrawLayers(layers);
	//	}

	//	public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
	//	{
	//		base.ModifyHitByNPC(npc, ref damage, ref crit);
	//	}

	//	public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
	//	{
	//		base.ModifyHitByProjectile(proj, ref damage, ref crit);
	//	}

	//	public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
	//	{
	//		base.ModifyHitNPC(item, target, ref damage, ref knockback, ref crit);
	//	}

	//	public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
	//	{
	//		base.ModifyHitNPCWithProj(proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
	//	}

	//	public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
	//	{
	//		base.ModifyHitPvp(item, target, ref damage, ref crit);
	//	}

	//	public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
	//	{
	//		base.ModifyHitPvpWithProj(proj, target, ref damage, ref crit);
	//	}

	//	public override void ModifyScreenPosition()
	//	{
	//		base.ModifyScreenPosition();
	//	}

	//	public override void ModifyZoom(ref float zoom)
	//	{
	//		base.ModifyZoom(ref zoom);
	//	}

	//	public override void NaturalLifeRegen(ref float regen)
	//	{
	//		base.NaturalLifeRegen(ref regen);
	//	}

	//	public override void OnEnterWorld(Player player)
	//	{
	//		base.OnEnterWorld(player);
	//	}

	//	public override void OnHitAnything(float x, float y, Entity victim)
	//	{
	//		base.OnHitAnything(x, y, victim);
	//	}

	//	public override void OnHitByNPC(NPC npc, int damage, bool crit)
	//	{
	//		base.OnHitByNPC(npc, damage, crit);
	//	}

	//	public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
	//	{
	//		base.OnHitByProjectile(proj, damage, crit);
	//	}

	//	public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
	//	{
	//		base.OnHitNPC(item, target, damage, knockback, crit);
	//	}

	//	public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
	//	{
	//		base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
	//	}

	//	public override void OnHitPvp(Item item, Player target, int damage, bool crit)
	//	{
	//		base.OnHitPvp(item, target, damage, crit);
	//	}

	//	public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
	//	{
	//		base.OnHitPvpWithProj(proj, target, damage, crit);
	//	}

	//	public override void OnRespawn(Player player)
	//	{
	//		base.OnRespawn(player);
	//	}

	//	public override void PlayerConnect(Player player)
	//	{
	//		base.PlayerConnect(player);
	//	}

	//	public override void PlayerDisconnect(Player player)
	//	{
	//		base.PlayerDisconnect(player);
	//	}

	//	public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
	//	{
	//		base.PostHurt(pvp, quiet, damage, hitDirection, crit);
	//	}

	//	public override void PostItemCheck()
	//	{
	//		base.PostItemCheck();
	//	}

	//	public override void PostSavePlayer()
	//	{
	//		base.PostSavePlayer();
	//	}

	//	public override void PostUpdate()
	//	{
	//		base.PostUpdate();
	//	}

	//	public override void PostUpdateBuffs()
	//	{
	//		base.PostUpdateBuffs();
	//	}

	//	public override void PostUpdateEquips()
	//	{
	//		base.PostUpdateEquips();
	//	}

	//	public override void PostUpdateMiscEffects()
	//	{
	//		base.PostUpdateMiscEffects();
	//	}

	//	public override void PostUpdateRunSpeeds()
	//	{
	//		base.PostUpdateRunSpeeds();
	//	}

	//	public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
	//	{
	//		return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
	//	}

	//	public override bool PreItemCheck()
	//	{
	//		return base.PreItemCheck();
	//	}

	//	public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
	//	{
	//		return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
	//	}

	//	public override void PreSaveCustomData()
	//	{
	//		base.PreSaveCustomData();
	//	}

	//	public override void PreSavePlayer()
	//	{
	//		base.PreSavePlayer();
	//	}

	//	public override void PreUpdate()
	//	{
	//		base.PreUpdate();
	//	}

	//	public override void PreUpdateBuffs()
	//	{
	//		base.PreUpdateBuffs();
	//	}

	//	public override void PreUpdateMovement()
	//	{
	//		base.PreUpdateMovement();
	//	}

	//	public override void ProcessTriggers(TriggersSet triggersSet)
	//	{
	//		base.ProcessTriggers(triggersSet);
	//	}

	//	public override void ReceiveCustomBiomes(BinaryReader reader)
	//	{
	//		base.ReceiveCustomBiomes(reader);
	//	}

	//	public override void ResetEffects()
	//	{
	//		base.ResetEffects();
	//	}

	//	public override TagCompound Save()
	//	{
	//		return base.Save();
	//	}

	//	public override void SendClientChanges(ModPlayer clientPlayer)
	//	{
	//		base.SendClientChanges(clientPlayer);
	//	}

	//	public override void SendCustomBiomes(BinaryWriter writer)
	//	{
	//		base.SendCustomBiomes(writer);
	//	}

	//	public override void SetControls()
	//	{
	//		base.SetControls();
	//	}

	//	public override void SetupStartInventory(IList<Item> items)
	//	{
	//		base.SetupStartInventory(items);
	//	}

	//	public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
	//	{
	//		return base.ShiftClickSlot(inventory, context, slot);
	//	}

	//	public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	//	{
	//		return base.Shoot(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
	//	}

	//	public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
	//	{
	//		base.SyncPlayer(toWho, fromWho, newPlayer);
	//	}

	//	public override string ToString()
	//	{
	//		return base.ToString();
	//	}

	//	public override void UpdateBadLifeRegen()
	//	{
	//		base.UpdateBadLifeRegen();
	//	}

	//	public override void UpdateBiomes()
	//	{
	//		base.UpdateBiomes();
	//	}

	//	public override void UpdateBiomeVisuals()
	//	{
	//		base.UpdateBiomeVisuals();
	//	}

	//	public override void UpdateDead()
	//	{
	//		base.UpdateDead();
	//	}

	//	public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
	//	{
	//		base.UpdateEquips(ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
	//	}

	//	public override void UpdateLifeRegen()
	//	{
	//		base.UpdateLifeRegen();
	//	}

	//	public override void UpdateVanityAccessories()
	//	{
	//		base.UpdateVanityAccessories();
	//	}

	//	public override float UseTimeMultiplier(Item item)
	//	{
	//		return base.UseTimeMultiplier(item);
	//	}
	//}

	/// <summary>
	/// Holds player-entity data and handles it
	/// </summary>
	public class ModifierPlayer : ModPlayer
	{
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
			int manaBlock = (int) Math.Ceiling(damage * ManaShield) * 2;
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
