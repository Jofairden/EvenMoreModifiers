namespace Loot.Modifiers.WeaponModifiers
{
	public class DebuffCursedInferno : WeaponDebuffModifier
	{
		public override int BuffType => Terraria.ID.BuffID.CursedInferno;
		public override int BuffTime => 180;
	}
}
