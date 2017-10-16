using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines a modifier that affects the player
	/// </summary>
	public sealed class PlayerModifier : Modifier
	{
		public PlayerModifier(string name) : base(name)
		{
		}
	}
}
