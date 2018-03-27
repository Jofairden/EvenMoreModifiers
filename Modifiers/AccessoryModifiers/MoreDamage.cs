using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.System;
using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Modifiers.AccessoryModifiers
{
	public class MoreDamage : AccessoryModifier
	{
		public override ModifierEffectTooltipLine[] Description => new[]
		{
			new ModifierEffectTooltipLine { Text = "Player deals 100% more damage", Color =  Color.SlateGray},
		};

		public override float RarityLevel => 5f;

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			player.magicDamage *= 2f;
			player.meleeDamage *= 2f;
			player.rangedDamage *= 2f;
			player.minionDamage *= 2f;
		}
	}
}
