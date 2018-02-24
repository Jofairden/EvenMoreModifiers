using Loot.Modifiers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Loot
{
	/// <summary>
	/// Holds player-entity data and handles it
	/// </summary>
	public class EMMPlayer : ModPlayer
	{
		public static EMMPlayer PlayerInfo(Player player) => player.GetModPlayer<EMMPlayer>();

		public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
		{
			ModifierContext ctx = new ModifierContext
			{
				Player = player,
				CustomData = new Dictionary<string, object>()
				{
					{"EMMPlayer", this}
				},
				Method = ModifierContextMethod.UpdateItem
			};

			foreach (var i in player.inventory.Where(x => !x.IsAir))
			{
				ctx.Item = i;
				EMMItem.GetItemInfo(i)?.Modifier?.UpdateItem(ctx);
			}

			foreach (var i in player.armor.Where(x => !x.IsAir))
			{
				ctx.Item = i;
				EMMItem.GetItemInfo(i)?.Modifier?.UpdateItem(ctx, true);
			}
		}
	}
}
