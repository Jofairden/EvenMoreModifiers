using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier that affects the player
	/// </summary>
	public sealed class PlayerModifier : Modifier
	{
		public PlayerModifier(string name, string description = null, ModifierApplyDelegate applyDelegate = null) 
			: base(name, description, applyDelegate)
		{
		}
	}
}
