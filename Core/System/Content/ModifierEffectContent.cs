using Loot.Core.Attributes;
using Loot.Core.System.Core;
using System;
using System.Linq;
using System.Reflection;

namespace Loot.Core.System.Content
{
	public class ModifierEffectContent : BaseContent<ModifierEffect>
	{
		internal override bool CheckContentPiece(ModifierEffect contentPiece)
		{
			//verbose GetCustomAttributes call
			//because we call the constructor, it will throw our
			//validation exceptions on load instead on entering world
			var attributes = contentPiece
				.GetType()
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				.Select(x => x.GetCustomAttributes(false).OfType<DelegationPrioritizationAttribute>());

			foreach (var attribute in attributes.SelectMany(x => x))
			{
				Activator.CreateInstance(attribute.GetType(), attribute.DelegationPrioritization, attribute.DelegationLevel);
			}

			// If we reached this point, all was fine.
			return true;
		}
	}
}
