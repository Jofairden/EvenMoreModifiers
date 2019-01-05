namespace Loot.Core.Attributes
{
	public enum DelegationTarget
	{
		Initialize,
		PreUpdate,
		ResetEffects,
		UpdateLifeRegen,
		UpdateBadLifeRegen,
		PostUpdateEquips,
		PostUpdate,
		PreHurt,
		PostHurt,
		ModifyHitNPC,
		ModifyHitPvp,
		OnHitNPC,
		OnHitPvp,
		PreKill
	}
}
