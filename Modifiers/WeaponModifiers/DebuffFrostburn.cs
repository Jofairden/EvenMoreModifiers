namespace Loot.Modifiers.WeaponModifiers
{
	public class DebuffFrostburn : WeaponDebuffModifier
	{
		public override int BuffType => Terraria.ID.BuffID.Frostburn;
		public override int BuffTime => 240;
	}
}
