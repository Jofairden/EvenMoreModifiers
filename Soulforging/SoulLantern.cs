using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Soulforging
{
	internal class SoulLantern : ModItem
	{
		public override string Texture => "Loot/Placeholder";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Lantern");
		}

		public override void SetDefaults()
		{
			item.rare = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			int i = tooltips.FindIndex(x => x.mod == "Terraria" && x.Name == "ItemName");
			if (i != -1)
			{
				var player = Main.LocalPlayer.GetModPlayer<LootEssencePlayer>();
				tooltips.Insert(i+1, new TooltipLine(mod, "Loot: SoulLantern:BonusEssenceGain", $"You gain {player.BonusEssenceGain} bonus essence every 10 kills."));
				tooltips.Insert(i+2, new TooltipLine(mod, "Loot: SoulLantern:SoulWeapon", $"You have no soul weapon equipped and have no additional powers."));
				tooltips.Insert(i+3, new TooltipLine(mod, "Loot: SoulLantern:TotalEssence", $"You have collected {player.Essence} essence."));
			}
		}
	}
}
