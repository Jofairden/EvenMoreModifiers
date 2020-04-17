using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Loot.Api.Cubes
{
	/// <summary>
	/// Defines a magical cube
	/// A magical cube is used to change modifiers on an item
	/// </summary>
	public abstract class MagicalCube : ModItem
	{
		protected abstract int EssenceCraftCost { get; }

		protected abstract string CubeName { get; }
		protected virtual Color? OverrideNameColor => null;
		protected virtual TooltipLine ExtraTooltip => null;

		public sealed override void SetDefaults()
		{
			item.Size = new Vector2(36);
			item.maxStack = 999;
			item.consumable = false;
			SafeDefaults();
		}

		public override void SetStaticDefaults()
		{
			SafeStaticDefaults();
		}

		protected virtual void SafeStaticDefaults()
		{
		}

		protected virtual void SafeDefaults()
		{
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			base.ModifyTooltips(tooltips);

			var tts = tooltips.Where(x => x.mod.Equals("Terraria"));

			if (OverrideNameColor != null)
			{
				var itemName = tts.FirstOrDefault(x => x.Name.Equals("ItemName"));
				if (itemName != null)
				{
					itemName.overrideColor = OverrideNameColor.Value;
				}
			}

			if (ExtraTooltip != null)
			{
				var desc = tts.Last(x => x.Name.StartsWith("Tooltip"));
				if (desc != null)
				{
					tooltips.Insert(tooltips.IndexOf(desc) + 1, ExtraTooltip);
				}
			}
		}
	}
}
