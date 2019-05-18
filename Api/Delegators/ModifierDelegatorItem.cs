using System.Collections.ObjectModel;
using Loot.Hacks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Loot.Api.Delegators
{
	/// <summary>
	/// Calls GlobalItem hooks on modifiers
	/// </summary>
	internal sealed class ModifierDelegatorItem : GlobalItem
	{
		public override bool InstancePerEntity => false;
		public override bool CloneNewInstances => false;
		private CheatedItemHackGlobalItem GetActivatedModifierItem(Item item) => CheatedItemHackGlobalItem.Item(item);

		public override bool AltFunctionUse(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.AltFunctionUse(item, player);
			}

			bool b = base.AltFunctionUse(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.AltFunctionUse(item, player);
			}

			return b;
		}

		public override bool CanEquipAccessory(Item item, Player player, int slot)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.CanEquipAccessory(item, player, slot);
			}

			bool b = base.CanEquipAccessory(item, player, slot);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.CanEquipAccessory(item, player, slot);
			}

			return b;
		}

		public override bool? CanHitNPC(Item item, Player player, NPC target)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.CanHitNPC(item, player, target);
			}

			bool? b = base.CanHitNPC(item, player, target);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.CanHitNPC(item, player, target);
			}

			return b;
		}

		public override bool CanHitPvp(Item item, Player player, Player target)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.CanHitPvp(item, player, target);
			}

			bool b = base.CanHitPvp(item, player, target);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.CanHitPvp(item, player, target);
			}

			return b;
		}

		public override bool CanPickup(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.CanPickup(item, player);
			}

			bool b = base.CanPickup(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.CanPickup(item, player);
			}

			return b;
		}

		public override bool CanRightClick(Item item)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.CanRightClick(item);
			}

			bool b = base.CanRightClick(item);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.CanRightClick(item);
			}

			return b;
		}

		public override bool CanUseItem(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.CanUseItem(item, player);
			}

			bool b = base.CanUseItem(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.CanUseItem(item, player);
			}

			return b;
		}

		public override int ChoosePrefix(Item item, UnifiedRandom rand)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.ChoosePrefix(item, rand);
			}

			int p = base.ChoosePrefix(item, rand);
			if (p != -1)
			{
				return p;
			}

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				p = m.ChoosePrefix(item, rand);
				// TODO which modifier takes precedence ?
				if (p != -1)
				{
					return p;
				}
			}

			return -1;
		}

		public override bool ConsumeAmmo(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.ConsumeAmmo(item, player);
			}

			bool b = base.ConsumeAmmo(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.ConsumeAmmo(item, player);
			}

			return b;
		}

		public override bool ConsumeItem(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.ConsumeItem(item, player);
			}

			bool b = base.ConsumeItem(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.ConsumeItem(item, player);
			}

			return b;
		}

		public override Color? GetAlpha(Item item, Color lightColor)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.GetAlpha(item, lightColor);
			}

			Color? a = base.GetAlpha(item, lightColor);
			if (a.HasValue)
			{
				return a;
			}

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				a = m.GetAlpha(item, lightColor);
				// TODO which modifier takes precedence ?
				if (a.HasValue)
				{
					return a;
				}
			}

			return null;
		}

		public override void GetWeaponCrit(Item item, Player player, ref int crit)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.GetWeaponCrit(item, player, ref crit);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.GetWeaponCrit(item, player, ref crit);
			}
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.GetWeaponDamage(item, player, ref damage);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.GetWeaponDamage(item, player, ref damage);
			}
		}

		public override void GetWeaponKnockback(Item item, Player player, ref float knockback)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.GetWeaponKnockback(item, player, ref knockback);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.GetWeaponKnockback(item, player, ref knockback);
			}
		}

		public override void GrabRange(Item item, Player player, ref int grabRange)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.GrabRange(item, player, ref grabRange);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.GrabRange(item, player, ref grabRange);
			}
		}

		public override bool GrabStyle(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.GrabStyle(item, player);
			}

			bool b = base.GrabStyle(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.GrabStyle(item, player);
			}

			return b;
		}

		public override void HoldItem(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.HoldItem(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.HoldItem(item, player);
			}
		}

		public override bool HoldItemFrame(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.HoldItemFrame(item, player);
			}

			bool b = base.HoldItemFrame(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.HoldItemFrame(item, player);
			}

			return b;
		}

		public override void HoldStyle(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.HoldStyle(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.HoldStyle(item, player);
			}
		}

		public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.HorizontalWingSpeeds(item, player, ref speed, ref acceleration);
			}
		}

		public override bool ItemSpace(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.ItemSpace(item, player);
			}

			bool b = base.ItemSpace(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.ItemSpace(item, player);
			}

			return b;
		}

		public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.MeleeEffects(item, player, hitbox);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.MeleeEffects(item, player, hitbox);
			}
		}

		public override float MeleeSpeedMultiplier(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.MeleeSpeedMultiplier(item, player);
			}

			float mult = base.MeleeSpeedMultiplier(item, player);
			if (mult != 1f)
			{
				return mult;
			}

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				mult = m.MeleeSpeedMultiplier(item, player);
				// TODO which modifier takes precedence ?
				if (mult != 1f)
				{
					return mult;
				}
			}

			return 1f;
		}

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.ModifyHitNPC(item, player, target, ref damage, ref knockBack, ref crit);
			}
		}

		public override void ModifyHitPvp(Item item, Player player, Player target, ref int damage, ref bool crit)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.ModifyHitPvp(item, player, target, ref damage, ref crit);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.ModifyHitPvp(item, player, target, ref damage, ref crit);
			}
		}

		// These are handled by EMMItem

		//public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		//{
		//	base.ModifyTooltips(item, tooltips);

		//	ModifierPool pool = EMMItem.GetPool(item);
		//	if (pool != null)
		//		foreach (Modifier m in pool.ActiveModifiers)
		//			m.ModifyTooltips(item, tooltips);
		//}

		//public override void NetReceive(Item item, BinaryReader reader)
		//{
		//	base.NetReceive(item, reader);

		//	ModifierPool pool = EMMItem.GetPool(item);
		//	if (pool != null)
		//		foreach (Modifier m in pool.ActiveModifiers)
		//			m.NetReceive(item, reader);
		//}

		//public override void NetSend(Item item, BinaryWriter writer)
		//{
		//	base.NetSend(item, writer);

		//	ModifierPool pool = EMMItem.GetPool(item);
		//	if (pool != null)
		//		foreach (Modifier m in pool.ActiveModifiers)
		//			m.NetSend(item, writer);
		//}

		public override bool NewPreReforge(Item item)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.NewPreReforge(item);
			}

			bool b = base.NewPreReforge(item);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.NewPreReforge(item);
			}

			return b;
		}

		public override void OnCraft(Item item, Recipe recipe)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.OnCraft(item, recipe);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.OnCraft(item, recipe);
			}
		}

		public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.OnHitNPC(item, player, target, damage, knockBack, crit);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.OnHitNPC(item, player, target, damage, knockBack, crit);
			}
		}

		public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.OnHitPvp(item, player, target, damage, crit);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.OnHitPvp(item, player, target, damage, crit);
			}
		}

		public override bool OnPickup(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.OnPickup(item, player);
			}

			bool b = base.OnPickup(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.OnPickup(item, player);
			}

			return b;
		}

		public override void PickAmmo(Item item, Player player, ref int type, ref float speed, ref int damage, ref float knockback)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.PickAmmo(item, player, ref type, ref speed, ref damage, ref knockback);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.PickAmmo(item, player, ref type, ref speed, ref damage, ref knockback);
			}
		}

		public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
			}
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
			}
		}

		public override void PostDrawTooltip(Item item, ReadOnlyCollection<DrawableTooltipLine> lines)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.PostDrawTooltip(item, lines);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.PostDrawTooltip(item, lines);
			}
		}

		public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.PostDrawTooltipLine(item, line);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.PostDrawTooltipLine(item, line);
			}
		}

		public override void PostReforge(Item item)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.PostReforge(item);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.PostReforge(item);
			}
		}

		public override void PostUpdate(Item item)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.PostUpdate(item);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.PostUpdate(item);
			}
		}

		public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
			}

			bool b = base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
			}

			return b;
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
			}

			bool b = base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
			}

			return b;
		}

		public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.PreDrawTooltip(item, lines, ref x, ref y);
			}

			bool b = base.PreDrawTooltip(item, lines, ref x, ref y);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.PreDrawTooltip(item, lines, ref x, ref y);
			}

			return b;
		}

		public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.PreDrawTooltipLine(item, line, ref yOffset);
			}

			bool b = base.PreDrawTooltipLine(item, line, ref yOffset);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.PreDrawTooltipLine(item, line, ref yOffset);
			}

			return b;
		}

		public override bool ReforgePrice(Item item, ref int reforgePrice, ref bool canApplyDiscount)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return base.ReforgePrice(item, ref reforgePrice, ref canApplyDiscount);
			}

			bool b = base.ReforgePrice(item, ref reforgePrice, ref canApplyDiscount);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.ReforgePrice(item, ref reforgePrice, ref canApplyDiscount);
			}

			return b;
		}

		public override void RightClick(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.RightClick(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.RightClick(item, player);
			}
		}

		public override void SetDefaults(Item item)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.SetDefaults(item);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.SetDefaults(item);
			}
		}

		public override bool Shoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
			}

			;

			bool b = base.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.Shoot(item, player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
			}

			return b;
		}

		public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, Main.LocalPlayer))
			{
				return;
			}

			base.Update(item, ref gravity, ref maxFallSpeed);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.Update(item, ref gravity, ref maxFallSpeed);
			}
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.UpdateAccessory(item, player, hideVisual);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.UpdateAccessory(item, player, hideVisual);
			}
		}

		public override void UpdateEquip(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.UpdateEquip(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.UpdateEquip(item, player);
			}
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.UpdateInventory(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.UpdateInventory(item, player);
			}
		}

		public override bool UseItem(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.UseItem(item, player);
			}

			bool b = base.UseItem(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.UseItem(item, player);
			}

			return b;
		}

		public override bool UseItemFrame(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.UseItemFrame(item, player);
			}

			bool b = base.UseItemFrame(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				b &= m.UseItemFrame(item, player);
			}

			return b;
		}

		public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.UseItemHitbox(item, player, ref hitbox, ref noHitbox);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.UseItemHitbox(item, player, ref hitbox, ref noHitbox);
			}
		}

		public override void UseStyle(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.UseStyle(item, player);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.UseStyle(item, player);
			}
		}

		public override float UseTimeMultiplier(Item item, Player player)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return base.UseTimeMultiplier(item, player);
			}

			float f = base.UseTimeMultiplier(item, player);
			if (f != 1f)
			{
				return f;
			}

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				f = m.UseTimeMultiplier(item, player);
				// TODO which modifier takes precedence ?
				if (f != 1f)
				{
					return f;
				}
			}

			return f;
		}

		public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
		{
			if (GetActivatedModifierItem(item).ShouldBeIgnored(item, player))
			{
				return;
			}

			base.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);

			foreach (Modifier.Modifier m in LootModItem.GetActivePool(item))
			{
				m.VerticalWingSpeeds(item, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
			}
		}
	}
}
