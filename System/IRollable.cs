namespace Loot.System
{
	public interface IRollable
	{
		// float RollChance
		bool CanRoll(ModifierContext ctx);
		bool CanApplyCraft(ModifierContext ctx);
		bool CanApplyPickup(ModifierContext ctx);
		bool CanApplyReforge(ModifierContext ctx);
	}
}
