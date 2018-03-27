namespace Loot.Modifiers.WeaponModifiers
{
	public class DebuffPoison : WeaponDebuffModifier
	{
		public override int BuffType => Terraria.ID.BuffID.Poisoned;
		public override int BuffTime => 480;
	}
}
