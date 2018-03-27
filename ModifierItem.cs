using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Loot.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Loot
{
	/// <summary>
	/// Calls GlobalItem hooks on modifiers
	/// </summary>
	public sealed class ModifierItem : GlobalItem
	{
		public override bool InstancePerEntity => false;
		public override bool CloneNewInstances => false;

		public override bool AltFunctionUse(Item item, Player player)
		{
			bool b = base.AltFunctionUse(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.AltFunctionUse(item, player);

			return b;
		}

		public override bool CanEquipAccessory(Item item, Player player, int slot)
		{
			bool b = base.CanEquipAccessory(item, player, slot);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.CanEquipAccessory(item, player, slot);

			return b;
		}

		public override bool? CanHitNPC(Item item, Player player, NPC target)
		{
			bool? b = base.CanHitNPC(item, player, target);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.CanHitNPC(item, player, target);

			return b;
		}

		public override bool CanHitPvp(Item item, Player player, Player target)
		{
			bool b = base.CanHitPvp(item, player, target);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.CanHitPvp(item, player, target);

			return b;
		}

		public override bool CanPickup(Item item, Player player)
		{
			bool b = base.CanPickup(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.CanPickup(item, player);

			return b;
		}

		public override bool CanRightClick(Item item)
		{
			bool b = base.CanRightClick(item);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.CanRightClick(item);

			return b;
		}

		public override bool CanUseItem(Item item, Player player)
		{
			bool b = base.CanUseItem(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.CanUseItem(item, player);

			return b;
		}

		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			int p = base.ChoosePrefix(item, rand);
			if (p != -1)
				return p;

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
				{
					p = m.ChoosePrefix(item, rand);
					// TODO which modifier takes precedence ?
					if (p != -1)
						return p;
				}

			return -1;
		}

		public override bool ConsumeAmmo(Item item, Player player)
		{
			bool b = base.ConsumeAmmo(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.ConsumeAmmo(item, player);

			return b;
		}

		public override bool ConsumeItem(Item item, Player player)
		{
			bool b = base.ConsumeItem(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.ConsumeItem(item, player);

			return b;
		}

		public override Color? GetAlpha(Item item, Color lightColor)
		{
			Color? a = base.GetAlpha(item, lightColor);
			if (a.HasValue)
				return a;

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
				{
					a = m.GetAlpha(item, lightColor);
					// TODO which modifier takes precedence ?
					if (a.HasValue)
						return a;
				}

			return null;
		}

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			base.GetWeaponCrit(item, player, ref crit);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.GetWeaponCrit(item, player, ref crit);
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.GetWeaponDamage(item, player, ref damage);
		}

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			base.GetWeaponKnockback(item, player, ref knockback);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.GetWeaponKnockback(item, player, ref knockback);
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			base.GrabRange(item, player, ref grabRange);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.GrabRange(item, player, ref grabRange);
		}

		public override bool GrabStyle(Item item, Player player)
		{
			bool b = base.GrabStyle(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.GrabStyle(item, player);

			return b;
		}

		public override void HoldItem(Item item, Player player)
		{
			base.HoldItem(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.HoldItem(item, player);
		}

		public override bool HoldItemFrame(Item item, Player player)
		{
			bool b = base.HoldItemFrame(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.HoldItemFrame(item, player);

			return b;
		}

		public override void HoldStyle(Item item, Player player)
		{
			base.HoldStyle(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.HoldStyle(item, player);
		}

		public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
		{
			base.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);
		}

		public override bool ItemSpace(Item item, Player player)
		{
			bool b = base.ItemSpace(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.ItemSpace(item, player);

			return b;
		}

		public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
		{
			base.MeleeEffects(item, player, hitbox);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.MeleeEffects(item, player, hitbox);
		}

		public override float MeleeSpeedMultiplier(Item item, Player player)
		{
			float mult = base.MeleeSpeedMultiplier(item, player);
			if (mult != 1f)
				return mult;

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
				{
					mult = m.MeleeSpeedMultiplier(item, player);
					// TODO which modifier takes precedence ?
					if (mult != 1f)
						return mult;
				}

			return 1f;
		}

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
			base.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);
		}

		public override void ModifyHitPvp(Item item, Player player, Player target, ref int damage, ref bool crit)
		{
			base.ModifyHitPvp(item, player, target, ref damage, ref crit);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.ModifyHitPvp(item, player, target, ref damage, ref crit);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			base.ModifyTooltips(item, tooltips);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.ModifyTooltips(item, tooltips);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			base.NetReceive(item, reader);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.NetReceive(item, reader);
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			base.NetSend(item, writer);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.NetSend(item, writer);
		}

		public override bool NewPreReforge(Item item)
		{
			bool b = base.NewPreReforge(item);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.NewPreReforge(item);

			return b;
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			base.OnCraft(item, recipe);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.OnCraft(item, recipe);
		}

		public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
		{
			base.OnHitNPC(item, player, target, damage, knockBack, crit);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.OnHitNPC(item, player, target, damage, knockBack, crit);
		}

		public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
		{
			base.OnHitPvp(item, player, target, damage, crit);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.OnHitPvp(item, player, target, damage, crit);
		}

		public override bool OnPickup(Item item, Player player)
		{
			bool b = base.OnPickup(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.OnPickup(item, player);

			return b;
		}

		public override void PickAmmo(Item item, Player player, ref int type, ref float speed, ref int damage, ref float knockback)
		{
			base.PickAmmo(item, player, ref type, ref speed, ref damage, ref knockback);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.PickAmmo(item, player, ref type, ref speed, ref damage, ref knockback);
		}

		public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
		}

		public override void PostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
		{
			base.PostDrawTooltip(item, lines);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.PostDrawTooltip(item, lines);
		}

		public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
		{
			base.PostDrawTooltipLine(item, line);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.PostDrawTooltipLine(item, line);
		}

		public override void PostReforge(Item item)
		{
			base.PostReforge(item);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.PostReforge(item);
		}

		public override void PostUpdate(Item item)
		{
			base.PostUpdate(item);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.PostUpdate(item);
		}

		public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			bool b = base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);

			return b;
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			bool b = base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);

			return b;
		}

		public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
		{
			bool b = base.PreDrawTooltip(item, lines, ref x, ref y);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.PreDrawTooltip(item, lines, ref x, ref y);

			return b;
		}

		public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
		{
			bool b = base.PreDrawTooltipLine(item, line, ref yOffset);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.PreDrawTooltipLine(item, line, ref yOffset);

			return b;
		}

		public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
		{
			bool b = base.ReforgePrice(item, ref reforgePrice, ref canApplyDiscount);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.ReforgePrice(item, ref reforgePrice, ref canApplyDiscount);

			return b;
		}

		public override void RightClick(Item item, Player player)
		{
			base.RightClick(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.RightClick(item, player);
		}

		public override void SetDefaults(Item item)
		{
			base.SetDefaults(item);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.SetDefaults(item);
		}

		public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			bool b = base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);

			return b;
		}

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			base.Update(item, ref gravity, ref maxFallSpeed);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.Update(item, ref gravity, ref maxFallSpeed);
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			base.UpdateAccessory(item, player, hideVisual);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.UpdateAccessory(item, player, hideVisual);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			base.UpdateEquip(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.UpdateEquip(item, player);
		}

		public override void UpdateInventory(Item item, Player player)
		{
			base.UpdateInventory(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.UpdateInventory(item, player);
		}

		public override bool UseItem(Item item, Player player)
		{
			bool b = base.UseItem(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.UseItem(item, player);

			return b;
		}

		public override bool UseItemFrame(Item item, Player player)
		{
			bool b = base.UseItemFrame(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					b &= m.UseItemFrame(item, player);

			return b;
		}

		public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
		{
			base.UseItemHitbox(item, player, ref hitbox, ref noHitbox);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.UseItemHitbox(item, player, ref hitbox, ref noHitbox);
		}

		public override void UseStyle(Item item, Player player)
		{
			base.UseStyle(item, player);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.UseStyle(item, player);
		}

		public override float UseTimeMultiplier(Item item, Player player)
		{
			float f = base.UseTimeMultiplier(item, player);
			if (f != 1f)
				return f;

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
				{
					f = m.UseTimeMultiplier(item, player);
					// TODO which modifier takes precedence ?
					if (f != 1f)
						return f;
				}

			return f;
		}

		public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			base.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);

			ModifierPool pool = EMMItem.GetPool(item);
			if (pool != null)
				foreach (Modifier m in pool.Active)
					m.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
		}
	}

}
