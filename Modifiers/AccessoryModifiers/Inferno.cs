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
	public class Inferno : AccessoryModifier
	{
		public override ModifierEffectTooltipLine[] Description => new[]
		{
			new ModifierEffectTooltipLine { Text = "Player has inferno", Color =  Color.IndianRed},
		};

		public override float RarityLevel => 3f;

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			player.inferno = true;
		}
	}
}
