namespace Loot.Modifiers.WeaponModifiers
{
	public class DebuffIchor : WeaponDebuffModifier
	{
		public override int BuffType => Terraria.ID.BuffID.Ichor;
		public override int BuffTime => 180;
	}
}
