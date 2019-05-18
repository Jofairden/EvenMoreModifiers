using System;
using Terraria.ModLoader;

namespace Loot.ModSupport
{
	internal class WingSlotSupport : ModSupport
	{
		public override string ModName => "WingSlot";

		public bool IsInvalid;

		private static bool WingSlotHandler()
			// TODO re-add the support (need WingSlot beta compatible)
			//=> Loot.Instance.CubeInterface.CurrentState is CubeUI uiState && uiState.Visible;
			=> true;

		public override bool CheckValidity(Mod mod)
		{
			IsInvalid = mod.Version < new Version(1, 6, 1);
			return !IsInvalid;
		}

		public override void AddClientSupport(Mod mod)
		{
			mod.Call("add", (Func<bool>)WingSlotHandler);
		}
	}
}
