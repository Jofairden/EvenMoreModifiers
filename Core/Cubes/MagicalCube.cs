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
		Basic
	}

	public abstract class MagicalCube : ModItem
	{
		protected abstract string CubeName { get; }
		protected abstract CubeTier Tier { get; }

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
			if (!CubeUI.Visible || CubeUI.Visible && Loot.Instance.cubeUI._rerollItemPanel.item.IsAir)
			{
				CubeUI.ToggleUI();
			}
			
			Loot.Instance.cubeUI._cubePanel.item.SetDefaults(item.type);
			Loot.Instance.cubeUI._cubePanel.RecalculateStack();
			
			// Must be after recalc, otherwise it affects the calculated stack
			item.stack++;
		}
	}
}