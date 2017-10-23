using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Modifiers;
using Terraria;

namespace Loot.Effects
{
	public class DebugEffect : ModifierEffect
	{
		public override string Description => "50% increased melee damage\nPlayer has inferno\nPlayer has godly defense";
		public override float Strength => 5f;

		public override void Apply(object target, Item item, string methodName)
		{
			Player player = target as Player;
			if (player != null && methodName == "UpdateEquip" && item.accessory)
			{
				player.meleeDamage += 0.5f;
				player.inferno = true;
				player.statDefense = 9999;

				Main.NewText($"{Strength}");
			}
		}
	}
}
