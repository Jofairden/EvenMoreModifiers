using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Essences
{
	abstract class TempItem : ModItem
	{
		public override string Texture => "Loot/Placeholder";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(GetType().Namespace + "." + Name);
		}
	}
}
