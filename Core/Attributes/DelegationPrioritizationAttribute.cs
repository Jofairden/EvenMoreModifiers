using System;

namespace Loot.Core.Attributes
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

	/// <summary>
	/// This attribute is used to set a custom prioritization for a delegation
	/// It allows you to customize at which point your delegation is called in the chain
	/// The end result is a prioritization list as follows:
	///		First part: all delegations prioritized as <see cref="F:DelegationPrioritization.Early"/>, order by their level
	///		Second part: all delegations with no custom prioritization (default)
	///		Third part: all delegations prioritized as <see cref="F:DelegationPrioritization.Late"/>.Late, order by their level
	/// To increase the force put into your prioritization, increase the delegation level
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public class DelegationPrioritizationAttribute : Attribute
	{
		public DelegationPrioritization DelegationPrioritization { get; }
		public int DelegationLevel { get; }

		public DelegationPrioritizationAttribute(DelegationPrioritization delegationPrioritization, int delegationLevel = 0)
		{
			if (delegationLevel < 0 || delegationLevel > 999)
			{
				throw new ArgumentOutOfRangeException(nameof(delegationLevel), delegationLevel, "delegationLevel must be within 0-999");
			}

			DelegationPrioritization = delegationPrioritization;
			DelegationLevel = delegationLevel;
		}
	}
}
