using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier that affects an item
	/// </summary>
	public sealed class ItemModifier : Modifier
	{
		public ItemModifier(string name, string description = null, ModifierApplyDelegate applyDelegate = null) 
			: base(name, description, applyDelegate)
		{
		}
	}
}
