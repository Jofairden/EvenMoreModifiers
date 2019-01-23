using System;
using Loot.UI.Core;
using Terraria.ModLoader;

namespace Loot.Ext.ModSupport
{
	internal class WingSlotSupporter : ModSupporter
	{
		public override string ModName => "WingSlot";

		public bool IsInvalid;

		private static bool WingSlotHandler()
			=> Loot.Instance.CubeInterface.CurrentState is CubeUI uiState && uiState.Visible;

		public override bool CheckValidity(Mod mod)
		{
			IsInvalid = mod.Version < new Version(1, 6, 1);
			return !IsInvalid;
		}

		public override void AddClientSupport(Mod mod)
		{
			mod.Call("add", (Func<bool>) WingSlotHandler);
		}
	}
}
