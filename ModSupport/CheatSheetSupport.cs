using System;
using Terraria.ModLoader;

namespace Loot.ModSupport
{
	internal class CheatSheetSupport : ModSupport
	{
		public override string ModName => "CheatSheet";

		public override bool CheckValidity(Mod mod)
			=> mod.Version >= new Version(0, 4, 3, 1);
	}
}
