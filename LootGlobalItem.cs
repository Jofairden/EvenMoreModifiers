using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// Holds entity based data
	/// </summary>
	class EMMItem : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;
	}

	/// <summary>
	/// Handles data globally
	/// </summary>
	class LootItem : GlobalItem
	{
	}
}
