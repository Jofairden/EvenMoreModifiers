using System.Reflection;

namespace Loot.Core.Caching
{
	public class OrderedDelegationEntry
	{
		public MethodInfo MethodInfo { get; set; }
		public ModifierEffect Effect { get; set; }
	}
}
