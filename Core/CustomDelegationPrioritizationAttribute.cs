using System;

namespace Loot.Core
{
	public enum DelegationPrioritization
	{
		Early,
		Late
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public class DelegationPrioritizationAttribute : Attribute
	{
		public DelegationPrioritization DelegationPrioritization { get; }
		public int DelegationLevel { get; }

		public DelegationPrioritizationAttribute(DelegationPrioritization delegationPrioritization, int delegationLevel = 0)
		{
			if (delegationLevel < 0 || delegationLevel > 999)
				throw new ArgumentOutOfRangeException(nameof(delegationLevel), delegationLevel, "delegationLevel must be within 0-999");

			DelegationPrioritization = delegationPrioritization;
			DelegationLevel = delegationLevel;
		}
	}
}
