using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Loot.Items;
using Terraria.DataStructures;

namespace Loot
{
    public class MPlayer : ModPlayer
    {
        public bool holdingCursed = false;
        public float lightStrength = 0;
        public int critDamageBoost = 0;
        public int luck = 0;
        public int bonusImmunity = 0;
        public int dodgeChance = 0;
        public int poisonChance = 0;
        public int onFireChance = 0;
        public int frostburnChance = 0;
        public int confusionChance = 0;
        public int infernoChance = 0;
        public int ichorChance = 0;
        public int subLifeRegen = 0;
        public int bonusDamageToMaxLife = 0;
        public int miracleChance = 0;
        public int percentDamageToMana = 0;
        public int percentDefBoost = 0;

        public override void ResetEffects()
        {
            lightStrength = 0;
            critDamageBoost = 0;
            luck = 0;
            bonusImmunity = 0;
            dodgeChance = 0;
            poisonChance = 0;
            onFireChance = 0;
            frostburnChance = 0;
            confusionChance = 0;
            infernoChance = 0;
            ichorChance = 0;
            bonusDamageToMaxLife = 0;
            miracleChance = 0;
            percentDamageToMana = 0;
            percentDefBoost = 0;
        }
        public override void UpdateLifeRegen()
        {
            while(subLifeRegen >= 30)
            {
                player.lifeRegen++;
                subLifeRegen -= 30;
            }
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
        public override void PostUpdateEquips()
        {
            if(lightStrength > 0)
            {
                Lighting.AddLight(player.Center, .15f * lightStrength, .15f * lightStrength, .15f * lightStrength);
            }
            //held item updates
            Item item = Main.item[player.selectedItem];
            if (item.type == 0)
            {
                return;
            }
            player.statDefense = (int)(player.statDefense * (1 + percentDefBoost / 100f));

        }
        public override void PostUpdate()
        {
            base.PostUpdate();
        }
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            int immuneFrames = bonusImmunity;
            if(damage <= 1)
            {
                immuneFrames /= 2;
            }
            if (player.immuneTime > 0)
            {
                player.immuneTime += immuneFrames;
            }
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if(dodgeChance > 40)
            {
                dodgeChance = 40;
            }
            if (Main.rand.Next(100) < dodgeChance)
            {
                player.NinjaDodge();
                return false;
            }
            if(percentDamageToMana > 0 && player.statMana > 0)
            {
                int damageToMana = Math.Min(player.statMana/2, (int)(damage * (percentDamageToMana / 100f)));
                if(percentDamageToMana > 0 && damageToMana == 0)
                {
                    damageToMana = 1;
                }
                player.statMana -= damageToMana*2;
                if (damageToMana > 0)
                {
                    player.manaRegenDelay = Math.Max(player.manaRegenDelay, 120);
                }
                damage -= damageToMana;
            }
            return base.PreHurt(pvp, quiet, ref damage, ref hitDirection, ref crit, ref customDamage, ref playSound, ref genGore, ref damageSource);
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (crit)
            {
                damage = (int)(damage * (1 + critDamageBoost / 200.0));
            }
            if(target.life == target.lifeMax)
            {
                damage = (int)(damage * (1 + bonusDamageToMaxLife / 100.0));
            }
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (crit)
            {
                damage = (int)(damage * (1 + critDamageBoost / 200.0));
            }
            if (target.life == target.lifeMax)
            {
                damage = (int)(damage * (1 + bonusDamageToMaxLife / 100f));
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            //debuffs
            if (Main.rand.Next(100) < poisonChance)
            {
                target.AddBuff(BuffID.Poisoned, 480);
            }
            if (Main.rand.Next(100) < onFireChance)
            {
                target.AddBuff(BuffID.OnFire, 300);
            }
            if (Main.rand.Next(100) < frostburnChance)
            {
                target.AddBuff(BuffID.Frostburn, 240);
            }
            if (Main.rand.Next(100) < confusionChance)
            {
                target.AddBuff(BuffID.Confused, 120);
            }
            if (Main.rand.Next(100) < infernoChance)
            {
                target.AddBuff(BuffID.CursedInferno, 180);
            }
            if (Main.rand.Next(100) < ichorChance)
            {
                target.AddBuff(BuffID.Ichor, 180);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            //debuffs
            if (Main.rand.Next(100) < poisonChance)
            {
                target.AddBuff(BuffID.Poisoned, 300);
            }
            if (Main.rand.Next(100) < onFireChance)
            {
                target.AddBuff(BuffID.OnFire, 300);
            }
            if (Main.rand.Next(100) < frostburnChance)
            {
                target.AddBuff(BuffID.Frostburn, 240);
            }
            if (Main.rand.Next(100) < confusionChance)
            {
                target.AddBuff(BuffID.Poisoned, 120);
            }
            if (Main.rand.Next(100) < infernoChance)
            {
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (Main.rand.Next(100) < Math.Min(miracleChance, 80))
            {
                player.statLife = 1;
                return false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
        }
    }
}
