namespace Loot.Modifiers.WeaponModifiers
{
	public class DebuffConfuse : WeaponDebuffModifier
	{
		public override int BuffType => Terraria.ID.BuffID.Confused;
		public override int BuffTime => 120;
	}
}
