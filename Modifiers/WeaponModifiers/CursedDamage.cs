using System;
using System.Linq;
using Loot.Core.Attributes;
using Loot.Core.System.Modifier;
using Loot.Ext;
using Terraria;
using Terraria.ID;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CursedEffect : ModifierEffect
	{
		public int CurseCount;

		public override void ResetEffects(ModifierPlayer player)
		{
			CurseCount = 0;
		}

		// There are two ways to delegate your method,
		// you can either specify by enum (safest) like here
		// or by string (see below)
		[AutoDelegation(DelegationTarget.PostUpdateEquips)]
		private void CurseHolding(ModifierPlayer player)
		{
			Item checkItem = Main.mouseItem != null && !Main.mouseItem.IsAir ? Main.mouseItem : player.player.HeldItem;

			if (checkItem != null && !checkItem.IsAir && checkItem.IsWeapon())
			{
				if (ActivatedModifierItem.Item(checkItem).IsActivated)
				{
					int c = EMMItem.GetActivePool(checkItem).Count(x => x.GetType() == typeof(CursedDamage));
					if (c > 0)
					{
						ModifierPlayer.Player(player.player).GetEffect<CursedEffect>().CurseCount += c;
					}
				}
			}
		}

		// The alternative way is providing the target name yourself
		// in the form of a string. It can be preceded by "On"
		// but it may also be left out.
		[AutoDelegation("OnUpdateBadLifeRegen")]
		private void Curse(ModifierPlayer player)
		{
			if (CurseCount > 0 && !player.player.buffImmune[BuffID.Cursed])
			{
				if (player.player.lifeRegen > 0)
				{
					player.player.lifeRegen = 0;
				}

				player.player.lifeRegen -= 2 * CurseCount;
				player.player.lifeRegenTime = 0;
			}
		}
	}

	[UsesEffect(typeof(CursedEffect))]
	public class CursedDamage : WeaponModifier
	{
		public override ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% damage, but you are cursed while holding this item");
		}

		public override ModifierPropertiesBuilder GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item)
				.WithMinMagnitude(5f)
				.WithMaxMagnitude(15f)
				.IsUniqueModifier(true);
		}

		public override bool CanRoll(ModifierContext ctx)
		{
			return base.CanRoll(ctx) && ctx.Method != ModifierContextMethod.SetupStartInventory;
		}

		public override void UpdateInventory(Item item, Player player)
		{
			// todo Is this good? Or do we want to change ModifierItem?
			//if (ActivatedModifierItem.Item(item).IsActivated)
			//{
			//	ModifierPlayer.Player(player).GetEffect<CursedEffect>().CurseCount++;
			//}
		}

		public override void GetWeaponDamage(Item item, Player player, ref int damage)
		{
			base.GetWeaponDamage(item, player, ref damage);
			damage = (int) Math.Ceiling(damage * (1 + Properties.RoundedPower / 100f));
		}
	}
}
