using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Loot.Soulforging
{
	class EssenceCommand : Terraria.ModLoader.ModCommand
	{
		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (int.TryParse(args[0], out int essence))
			{
				caller.Player.GetModPlayer<LootEssencePlayer>().GainEssence(essence);
			}
		}

		public override string Command => "addEssence";

		public override CommandType Type => CommandType.Chat;
	}
}
