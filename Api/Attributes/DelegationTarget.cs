namespace Loot.Api.Attributes
{
	/// <summary>
	/// Defines a target that can be used in conjunction with <see cref="AutoDelegation"/>
	/// for identifying the target event
	/// </summary>
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
