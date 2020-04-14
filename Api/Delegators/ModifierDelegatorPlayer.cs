using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Api.Loaders;
using Loot.Caching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Loot.Api.Delegators
{
	/// <summary>
	/// Holds player-entity data and handles it
	/// </summary>
	public class ModifierDelegatorPlayer : ModPlayer
	{
		private IList<ModifierEffect> _modifierEffects;

		public bool HasEffect(Type type) => _modifierEffects.Any(x => x.GetType() == type);
		public bool HasEffect<T>() where T : ModifierEffect => _modifierEffects.Any(x => x.GetType() == typeof(T));

		public ModifierEffect GetEffect(Type type)
		{
			return _modifierEffects.FirstOrDefault(x => x.GetType() == type);
		}

		public T GetEffect<T>() where T : ModifierEffect
		{
			return (T)_modifierEffects.FirstOrDefault(x => x.GetType() == typeof(T));
		}

		public static ModifierDelegatorPlayer GetPlayer(Player player) => player.GetModPlayer<ModifierDelegatorPlayer>();

		/*****************
		 *** OVERRIDES ***
		 *****************/
		//public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
		//{
		//	if (!mediumcoreDeath)
		//	{
		//		LootModWorld.WorldGenModifiersPass.GenerateModifiers(null, ModifierContextMethod.SetupStartInventory, items.Where(x => !x.IsAir && x.IsModifierRollableItem()), player);
		//	}
		//}

		//public override void OnEnterWorld(Player player)
		//{
		//	LootModWorld.WorldGenModifiersPass.GenerateModifiers(null, ModifierContextMethod.FirstLoad, player.inventory.Where(x => !x.IsAir && x.IsModifierRollableItem()).Concat(player.armor.Where(x => !x.IsAir && !x.IsModifierRollableItem())), player);
		//}

		public event Action<Player> OnRespawnEvent;
		public override void OnRespawn(Player player)
		{
			OnRespawnEvent?.Invoke(player);
		}

		public event Func<Item[], int, int, bool> ShiftClickSlotEvent;
		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			return ShiftClickSlotEvent?.Invoke(inventory, context, slot) ?? base.ShiftClickSlot(inventory, context, slot);
		}

		public delegate bool ModifyNurseHealEventRaiser(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText);
		public event ModifyNurseHealEventRaiser ModifyNurseHealEvent;
		public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
		{
			return ModifyNurseHealEvent?.Invoke(nurse, ref health, ref removeDebuffs, ref chatText) ?? base.ModifyNurseHeal(nurse, ref health, ref removeDebuffs, ref chatText);
		}

		public delegate void ModifyNursePriceEventRaiser(NPC nurse, int health, bool removeDebuffs, ref int price);
		public event ModifyNursePriceEventRaiser ModifyNursePriceEvent;
		public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price)
		{
			ModifyNursePriceEvent?.Invoke(nurse, health, removeDebuffs, ref price);
		}

		public event Action<NPC, int, bool, int> PostNurseHealEvent;
		public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
		{
			PostNurseHealEvent?.Invoke(nurse, health, removeDebuffs, price);
		}

		public event Action InitializeEvent;
		public override void Initialize()
		{
			// Initialize the effects list for this player
			_modifierEffects = new List<ModifierEffect>();

			// Need to initialize with a fresh set of new effect instances
			foreach (var effect in ContentLoader.ModifierEffect.Content.Select(x => x.Value))
			{
				var clone = (ModifierEffect)effect.Clone();
				clone.DelegatorPlayer = this;
				clone.OnInitialize();
				_modifierEffects.Add(clone);
			}

			InitializeEvent?.Invoke();
		}

		public event Action UpdateAutopauseEvent;
		public override void UpdateAutopause()
		{
			UpdateAutopauseEvent?.Invoke();
		}

		public event Action PreUpdateEvent;
		public override void PreUpdate()
		{
			PreUpdateEvent?.Invoke();
		}

		public event Action<TriggersSet> ProcessTriggersEvent;
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			ProcessTriggersEvent?.Invoke(triggersSet);
		}

		public event Action SetControlsEvent;
		public override void SetControls()
		{
			SetControlsEvent?.Invoke();
		}

		public event Action PreUpdateBuffsEvent;
		public override void PreUpdateBuffs()
		{
			PreUpdateBuffsEvent?.Invoke();
		}

		public event Action PostUpdateBuffsEvent;
		public override void PostUpdateBuffs()
		{
			PostUpdateBuffsEvent?.Invoke();
		}

		public delegate void UpdateEquipsEventRaiser(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff);
		public event UpdateEquipsEventRaiser UpdateEquipsEvent;
		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			UpdateEquipsEvent?.Invoke(ref wallSpeedBuff, ref tileSpeedBuff, ref tileRangeBuff);
		}

		public event Action ResetEffectsEvent;
		public override void ResetEffects()
		{
			if (player.GetModPlayer<ModifierCachePlayer>().Ready)
			{
				ResetEffectsEvent?.Invoke();
			}
		}

		public event Action UpdateDeadEvent;
		public override void UpdateDead()
		{
			UpdateDeadEvent?.Invoke();
		}

		public event Action UpdateLifeRegenEvent;
		public override void UpdateLifeRegen()
		{
			UpdateLifeRegenEvent?.Invoke();
		}

		public delegate void NaturalLifeRegenEventRaiser(ref float regen);
		public event NaturalLifeRegenEventRaiser NaturalLifeRegenEvent;
		public override void NaturalLifeRegen(ref float regen)
		{
			NaturalLifeRegenEvent?.Invoke(ref regen);
		}

		public event Action<int, int, bool> SyncPlayerEvent;
		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			SyncPlayerEvent?.Invoke(toWho, fromWho, newPlayer);
		}

		public event Action<ModPlayer> SendClientChangesEvent;
		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			SendClientChangesEvent?.Invoke(clientPlayer);
		}

		public event Action UpdateBadLifeRegenEvent;
		public override void UpdateBadLifeRegen()
		{
			UpdateBadLifeRegenEvent?.Invoke();
		}

		public event Action PostUpdateEquipsEvent;
		public override void PostUpdateEquips()
		{
			PostUpdateEquipsEvent?.Invoke();
		}

		public event Action PostUpdateMiscEffectsEvent;
		public override void PostUpdateMiscEffects()
		{
			PostUpdateMiscEffectsEvent?.Invoke();
		}

		public event Action PostUpdateRunSpeedsEvent;
		public override void PostUpdateRunSpeeds()
		{
			PostUpdateRunSpeedsEvent?.Invoke();
		}

		public event Action PreUpdateMovementEvent;
		public override void PreUpdateMovement()
		{
			PreUpdateMovementEvent?.Invoke();
		}

		public event Action PostUpdateEvent;
		public override void PostUpdate()
		{
			PostUpdateEvent?.Invoke();
		}

		public event Action UpdateVanityAccessoriesEvent;
		public override void UpdateVanityAccessories()
		{
			UpdateVanityAccessoriesEvent?.Invoke();
		}

		public event Action FrameEffectsEvent;
		public override void FrameEffects()
		{
			FrameEffectsEvent?.Invoke();
		}

		public delegate bool PreHurtEventRaiser(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
		public event PreHurtEventRaiser PreHurtEvent;
		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			return PreHurtEvent?.Invoke(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource)
				   ?? base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
		}

		public event Action<bool, bool, double, int, bool> HurtEvent;
		public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			HurtEvent?.Invoke(pvp, quiet, damage, hitDirection, crit);
		}

		public event Action<bool, bool, double, int, bool> PostHurtEvent;
		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			PostHurtEvent?.Invoke(pvp, quiet, damage, hitDirection, crit);
		}

		public event Func<Item, NPC, bool?> CanHitNPCEvent;
		public override bool? CanHitNPC(Item item, NPC target)
		{
			return CanHitNPCEvent?.Invoke(item, target) ?? base.CanHitNPC(item, target);
		}

		public delegate void ModifyHitNPCEventRaiser(Item item, NPC target, ref int damage, ref float knockback, ref bool crit);
		public event ModifyHitNPCEventRaiser ModifyHitNPCEvent;
		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			ModifyHitNPCEvent?.Invoke(item, target, ref damage, ref knockback, ref crit);
		}

		public event Func<Item, Player, bool> CanHitPvpEvent;
		public override bool CanHitPvp(Item item, Player target)
		{
			return CanHitPvpEvent?.Invoke(item, target) ?? base.CanHitPvp(item, target);
		}

		public delegate void ModifyHitPvpEventRaiser(Item item, Player target, ref int damage, ref bool crit);
		public event ModifyHitPvpEventRaiser ModifyHitPvpEvent;
		public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
		{
			ModifyHitPvpEvent?.Invoke(item, target, ref damage, ref crit);
		}

		public event Action<Item, NPC, int, float, bool> OnHitNPCEvent;
		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			OnHitNPCEvent?.Invoke(item, target, damage, knockback, crit);
		}

		public event Func<Projectile, NPC, bool?> CanHitNPCWithProjEvent;
		public override bool? CanHitNPCWithProj(Projectile proj, NPC target)
		{
			return CanHitNPCWithProjEvent?.Invoke(proj, target) ?? base.CanHitNPCWithProj(proj, target);
		}

		public delegate void ModifyHitNPCWithProjEventRaiser(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection);
		public event ModifyHitNPCWithProjEventRaiser ModifyHitNPCWithProjEvent;
		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			ModifyHitNPCWithProjEvent?.Invoke(proj, target, ref damage, ref knockback, ref crit, ref hitDirection);
		}

		public event Action<Projectile, NPC, int, float, bool> OnHitNPCWithProjEvent;
		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			OnHitNPCWithProjEvent?.Invoke(proj, target, damage, knockback, crit);
		}

		public event Action<Item, Player, int, bool> OnHitPvpEvent;
		public override void OnHitPvp(Item item, Player target, int damage, bool crit)
		{
			OnHitPvpEvent?.Invoke(item, target, damage, crit);
		}

		public event Func<Projectile, Player, bool> CanHitPvpWithProjEvent;
		public override bool CanHitPvpWithProj(Projectile proj, Player target)
		{
			return CanHitPvpWithProjEvent?.Invoke(proj, target) ?? base.CanHitPvpWithProj(proj, target);
		}

		public delegate void ModifyHitPvpWithProjEventRaiser(Projectile proj, Player target, ref int damage, ref bool crit);
		public event ModifyHitPvpWithProjEventRaiser ModifyHitPvpWithProjEvent;
		public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
		{
			ModifyHitPvpWithProjEvent?.Invoke(proj, target, ref damage, ref crit);
		}

		public event Action<Projectile, Player, int, bool> OnHitPvpWithProjEvent;
		public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
		{
			OnHitPvpWithProjEvent?.Invoke(proj, target, damage, crit);
		}

		public delegate bool CanBeHitByNPCEventRaiser(NPC npc, ref int cooldownSlot);
		public event CanBeHitByNPCEventRaiser CanBeHitByNPCEvent;
		public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
		{
			return CanBeHitByNPCEvent?.Invoke(npc, ref cooldownSlot) ?? base.CanBeHitByNPC(npc, ref cooldownSlot);
		}

		public delegate void ModifyHitByNPCEventRaiser(NPC npc, ref int damage, ref bool crit);
		public event ModifyHitByNPCEventRaiser ModifyHitByNPCEvent;
		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{
			ModifyHitByNPCEvent?.Invoke(npc, ref damage, ref crit);
		}

		public delegate void ModifyWeaponDamageEventRaiser(Item item, ref float add, ref float mult, ref float flat);
		public event ModifyWeaponDamageEventRaiser ModifyWeaponDamageEvent;
		public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat)
		{
			ModifyWeaponDamageEvent?.Invoke(item, ref add, ref mult, ref flat);
		}

		public delegate void ModifyManaCostEventRaiser(Item item, ref float reduce, ref float mult);
		public event ModifyManaCostEventRaiser ModifyManaCostEvent;
		public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
		{
			ModifyManaCostEvent?.Invoke(item, ref reduce, ref mult);
		}

		public event Action<NPC, int, bool> OnHitByNPCEvent;
		public override void OnHitByNPC(NPC npc, int damage, bool crit)
		{
			OnHitByNPCEvent?.Invoke(npc, damage, crit);
		}

		public event Func<Projectile, bool> CanBeHitByProjectileEvent;
		public override bool CanBeHitByProjectile(Projectile proj)
		{
			return CanBeHitByProjectileEvent?.Invoke(proj) ?? base.CanBeHitByProjectile(proj);
		}

		public delegate void ModifyHitByProjectileEventRaiser(Projectile projectile, ref int damage, ref bool crit);
		public event ModifyHitByProjectileEventRaiser ModifyHitByProjectileEvent;
		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			ModifyHitByProjectileEvent?.Invoke(proj, ref damage, ref crit);
		}

		public event Action<Projectile, int, bool> OnHitByProjectileEvent;
		public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
		{
			OnHitByProjectileEvent?.Invoke(proj, damage, crit);
		}

		public delegate void CatchFishEventRaiser(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk);
		public event CatchFishEventRaiser CatchFishEvent;
		public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
			CatchFishEvent?.Invoke(fishingRod, bait, power, liquidType, poolSize, worldLayer, questFish, ref caughtType, ref junk);
		}

		public delegate void GetFishingLevelEventRaiser(Item fishingRod, Item bait, ref int fishingLevel);
		public event GetFishingLevelEventRaiser GetFishingLevelEvent;
		public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
		{
			GetFishingLevelEvent?.Invoke(fishingRod, bait, ref fishingLevel);
		}

		public event Action<float, List<Item>> AnglerQuestRewardEvent;
		public override void AnglerQuestReward(float rareMultiplier, List<Item> rewardItems)
		{
			AnglerQuestRewardEvent?.Invoke(rareMultiplier, rewardItems);
		}

		public event Action<List<int>> GetDyeTraderRewardEvent;
		public override void GetDyeTraderReward(List<int> rewardPool)
		{
			GetDyeTraderRewardEvent?.Invoke(rewardPool);
		}

		public delegate void DrawEffectsEventRaiser(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright);
		public event DrawEffectsEventRaiser DrawEffectsEvent;
		public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			DrawEffectsEvent?.Invoke(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
		}

		public delegate void ModifyDrawInfoEventRaiser(ref PlayerDrawInfo drawInfo);
		public event ModifyDrawInfoEventRaiser ModifyDrawInfoEvent;
		public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
		{
			ModifyDrawInfoEvent?.Invoke(ref drawInfo);
		}

		public event Action<List<PlayerLayer>> ModifyDrawLayersEvent;
		public override void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			ModifyDrawLayersEvent?.Invoke(layers);
		}

		public event Action<List<PlayerHeadLayer>> ModifyDrawHeadLayersEvent;
		public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
		{
			ModifyDrawHeadLayersEvent?.Invoke(layers);
		}

		public event Action ModifyScreenPositionEvent;
		public override void ModifyScreenPosition()
		{
			ModifyScreenPositionEvent?.Invoke();
		}

		public delegate void ModifyZoomEventRaiser(ref float zoom);
		public event ModifyZoomEventRaiser ModifyZoomEvent;
		public override void ModifyZoom(ref float zoom)
		{
			ModifyZoomEvent?.Invoke(ref zoom);
		}

		public delegate bool PreKillEventRaiser(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
		public event PreKillEventRaiser PreKillEvent;
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			return PreKillEvent?.Invoke(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource)
				   ?? base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
		}

		public event Action<double, int, bool, PlayerDeathReason> KillEvent;
		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
		{
			KillEvent?.Invoke(damage, hitDirection, pvp, damageSource);
		}

		public event Func<bool> PreItemCheckEvent;
		public override bool PreItemCheck()
		{
			return PreItemCheckEvent?.Invoke() ?? base.PreItemCheck();
		}

		public event Action PostItemCheckEvent;
		public override void PostItemCheck()
		{
			PostItemCheckEvent?.Invoke();
		}

		public event Func<Item, float> UseTimeMultiplierEvent;
		public override float UseTimeMultiplier(Item item)
		{
			return UseTimeMultiplierEvent?.Invoke(item) ?? base.UseTimeMultiplier(item);
		}

		public event Func<Item, float> MeleeSpeedMultiplierEvent;
		public override float MeleeSpeedMultiplier(Item item)
		{
			return MeleeSpeedMultiplierEvent?.Invoke(item) ?? base.MeleeSpeedMultiplier(item);
		}

		public delegate void GetHealLifeEventRaiser(Item item, bool quickHeal, ref int healValue);
		public event GetHealLifeEventRaiser GetHealLifeEvent;
		public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
		{
			GetHealLifeEvent?.Invoke(item, quickHeal, ref healValue);
		}

		public delegate void GetHealManaEventRaiser(Item item, bool quickHeal, ref int healValue);
		public event GetHealManaEventRaiser GetHealManaEvent;
		public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
		{
			GetHealManaEvent?.Invoke(item, quickHeal, ref healValue);
		}

		public delegate void GetWeaponKnockbackEventRaiser(Item item, ref float knockback);
		public event GetWeaponKnockbackEventRaiser GetWeaponKnockbackEvent;
		public override void GetWeaponKnockback(Item item, ref float knockback)
		{
			GetWeaponKnockbackEvent?.Invoke(item, ref knockback);
		}

		public delegate void GetWeaponCritEventRaiser(Item item, ref int crit);
		public event GetWeaponCritEventRaiser GetWeaponCritEvent;
		public override void GetWeaponCrit(Item item, ref int crit)
		{
			GetWeaponCritEvent?.Invoke(item, ref crit);
		}

		public event Func<Item, Item, bool> ConsumeAmmoEvent;
		public override bool ConsumeAmmo(Item weapon, Item ammo)
		{
			return ConsumeAmmoEvent?.Invoke(weapon, ammo) ?? base.ConsumeAmmo(weapon, ammo);
		}

		public event Action<Item, Item> OnConsumeAmmoEvent;
		public override void OnConsumeAmmo(Item weapon, Item ammo)
		{
			OnConsumeAmmoEvent?.Invoke(weapon, ammo);
		}

		public delegate bool ShootEventRaiser(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack);
		public event ShootEventRaiser ShootEvent;
		public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			return ShootEvent?.Invoke(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack)
				?? base.Shoot(item, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}

		public event Action<Item, Rectangle> MeleeEffectsEvent;
		public override void MeleeEffects(Item item, Rectangle hitbox)
		{
			MeleeEffectsEvent?.Invoke(item, hitbox);
		}

		public event Action<float, float, Entity> OnHitAnythingEvent;
		public override void OnHitAnything(float x, float y, Entity victim)
		{
			OnHitAnythingEvent?.Invoke(x, y, victim);
		}
	}
}
