using Terraria;

namespace Loot.Core.Cubes
{
	/// <summary>
	/// Defines a rerolling cube that opens the rerolling UI on right click
	/// The method <see cref="M:SetRollLogic"/> can be overridden to provide
	/// custom roll logic
	/// </summary>
	public abstract class RerollingCube : MagicalCube
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(CubeName);
			Tooltip.SetDefault("Press left control and right click to open cube UI" +
			                   "\nAllows rerolling modifiers of an item" +
			                   "\nSlotted cube is consumed upon use");
			SafeStaticDefaults();
		}

		public override void RightClick(Player player)
		{
			Loot.Instance.CubeRerollUI._cubePanel.ChangeItem(item.type);

			if (!Loot.Instance.CubeRerollUI.Visible || Loot.Instance.CubeRerollUI.Visible && Loot.Instance.CubeRerollUI._rerollItemPanel.item.IsAir)
			{
				Loot.Instance.CubeRerollUI.ToggleUI(Loot.Instance.CubeInterface, Loot.Instance.CubeRerollUI);
			}

			// Must be after recalc, otherwise it affects the calculated stack
			item.stack++;
		}

		public virtual void SetRollLogic(ItemRollProperties properties)
		{
		}
	}
}
