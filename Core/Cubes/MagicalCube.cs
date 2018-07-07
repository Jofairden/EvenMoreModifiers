using System.Collections.Generic;
using Loot.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public enum CubeTier
	{
		TierI,
		TierII,
		TierIII
	}

	public abstract class MagicalCube : ModItem
	{
		protected abstract string CubeName { get; }
		protected CubeTier Tier = CubeTier.TierIII;
		protected virtual bool DisplayTier => true;

		public static string GetTierText(CubeTier tier)
		{
			switch (tier)
			{
				default:
				case CubeTier.TierIII:
					return "Tier III";
				case CubeTier.TierII:
					return "Tier II";
				case CubeTier.TierI:
					return "Tier I";
			}
		}

//		public override string Texture => (GetType().Namespace + ".MagicalCube").Replace('.', '/');

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(CubeName);
			Tooltip.SetDefault("Press left control and right click to open cube UI" +
			                   "\nAllows rerolling modifiers of an item" +
			                   "\nSlotted cube is consumed upon use");
			SafeStaticDefaults();
		}

		public sealed override void SetDefaults()
		{
			item.Size = new Vector2(36);
			item.maxStack = 999;
			item.consumable = false;
			SafeDefaults();
		}

		protected virtual void SafeStaticDefaults()
		{
		}

		protected virtual void SafeDefaults()
		{
		}

		public override bool CanRightClick() => !PlayerInput.WritingText && Main.hasFocus && Main.keyState.IsKeyDown(Keys.LeftControl);

		public override void RightClick(Player player)
		{
			Loot.Instance.CubeRerollUI._cubePanel.item.SetDefaults(item.type);
			Loot.Instance.CubeRerollUI._cubePanel.RecalculateStack();

			if (!Loot.Instance.CubeRerollUI.Visible || Loot.Instance.CubeRerollUI.Visible && Loot.Instance.CubeRerollUI._rerollItemPanel.item.IsAir)
			{
				Loot.Instance.CubeRerollUI.ToggleUI(Loot.Instance.CubeInterface, Loot.Instance.CubeRerollUI);
			}

			// Must be after recalc, otherwise it affects the calculated stack
			item.stack++;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (DisplayTier)
			{
				TooltipLine ttl = new TooltipLine(mod, "Loot: CubeTier", $"{GetTierText(Tier)}")
				{
					overrideColor = Color.Cyan
				};
				tooltips.Add(ttl);
			}
		}
	}
}