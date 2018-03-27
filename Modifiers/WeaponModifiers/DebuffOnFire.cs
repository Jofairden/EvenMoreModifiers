namespace Loot.Modifiers.WeaponModifiers
{
	public class DebuffOnFire : WeaponDebuffModifier
	{
		public override int BuffType => Terraria.ID.BuffID.OnFire;
		public override int BuffTime => 300;
	}
}
