namespace Loot.Api.Attributes
{
	/// <summary>
	/// Specify when you want your delegation to happen in the chain
	/// </summary>
	public enum DelegationPrioritization
	{
		/// <summary>
		/// Specify that you want your delegation to happen early in the chain
		/// </summary>
		Early,

		/// <summary>
		/// Specify that you want your delegation to happen late in the chain
		/// </summary>
		Late
	}
}