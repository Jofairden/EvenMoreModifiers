using System;
using Terraria.ModLoader;

namespace Loot.Ext.ModSupport
{
	internal class CheatSheetSupporter : ModSupporter
	{
		public override string ModName => "CheatSheet";

		public override bool CheckValidity(Mod mod)
			=> mod.Version >= new Version(0, 4, 3, 1);

	}
}
