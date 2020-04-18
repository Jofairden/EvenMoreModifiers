using System.Linq;
using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Modifiers.Base;
using Terraria;
using Terraria.ID;

namespace Loot.Modifiers.WeaponModifiers
{
	public class CursedEffect : ModifierEffect
	{
		public int CurseCount;

		public override void ResetEffects()
		{
			CurseCount = 0;
		}

		// There are two ways to delegate your method,
		// you can either specify by enum (safest) like here
		// or by string (see below)
		[AutoDelegation(DelegationTarget.PostUpdateEquips)]
		private void CurseHolding()
		{
			Item checkItem = Main.mouseItem != null && !Main.mouseItem.IsAir ? Main.mouseItem : player.HeldItem;

			if (checkItem == null || checkItem.IsAir || !checkItem.IsWeapon()
			    || !LootModItem.GetInfo(checkItem).IsActivated)
				return;

			int c = LootModItem.GetActivePool(checkItem).Count(x => x.GetType() == typeof(CursedDamage));
			if (c > 0)
			{
				DelegatorPlayer.GetEffect<CursedEffect>().CurseCount += c;
			}
		}

		// The alternative way is providing the target name yourself
		// in the form of a string. It can be preceded by "On"
		// but it may also be left out.
		[AutoDelegation("OnUpdateBadLifeRegen")]
		private void Curse()
		{
			if (CurseCount <= 0 || player.buffImmune[BuffID.Cursed])
				return;

			if (player.lifeRegen > 0)
			{
				player.lifeRegen = 0;
			}

			player.lifeRegen -= 2 * CurseCount;
			player.lifeRegenTime = 0;
		}
	}

	[UsesEffect(typeof(CursedEffect))]
	public class CursedDamage : WeaponModifier
	{
		public override ModifierTooltipLine.ModifierTooltipBuilder GetTooltip()
		{
			return base.GetTooltip()
				.WithPositive($"+{Properties.RoundedPower}% damage, but you are cursed while holding this item");
		}

		public override ModifierProperties.ModifierPropertiesBuilder GetModifierProperties(Item item)
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

		public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float mult, ref float flat)
		{
			base.ModifyWeaponDamage(item, player, ref add, ref mult, ref flat);
			add += Properties.RoundedPower / 100f;
		}
	}
}
