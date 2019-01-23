using Loot.Core.System.Modifier;
using Loot.Ext;
using Terraria;

namespace Loot.Modifiers.Base
{
	/// <summary>
	/// Defines a modifier that can roll on a weapon item
	/// You can use this class and add to CanRoll by calling base.CanRoll(ctx) and then your own conditionals
	/// </summary>
	public abstract class WeaponModifier : Modifier
	{
		public static bool HasVanillaDamage(Item item)
			=> item.magic || item.melee || item.ranged || item.summon || item.thrown;

		public override bool CanRoll(ModifierContext ctx)
			=> ctx.Item.IsWeapon();

		public void UpdateShootSpeed(Item item, Player player, float speedXBefore, float speedYBefore, ref float speedX, ref float speedY, float percentModifier)
		{
			float tempXSpeed = speedX;
			float tempYSpeed = speedY;
			float diffX = tempXSpeed * percentModifier - speedXBefore;
			float diffY = tempYSpeed * percentModifier - speedYBefore;
			speedX += diffX;
			speedY += diffY;
		}

		public void UpdateUseKnockback(Item item, Player player, float knockbackBefore, ref float knockback)
		{
			float tempKnockback = knockback;
			GetWeaponKnockback(item, player, ref tempKnockback);
			float diff = tempKnockback - knockbackBefore;
			knockback += diff;
		}

		public void UpdateUseDamage(Item item, Player player, int damageBefore, ref int damage)
		{
			int tempDamage = damage;
			GetWeaponDamage(item, player, ref tempDamage);
			int diff = tempDamage - damageBefore;
			damage += diff;
		}
	}
}
