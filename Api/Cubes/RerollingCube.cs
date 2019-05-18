using Loot.Api.Strategy;
using Loot.UI;
using Loot.UI.Tabs.Cubing;
using Terraria;

namespace Loot.Api.Cubes
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
			var gui = Loot.Instance.GuiState;
			gui.ToggleUI(Loot.Instance.GuiInterface);
			// Force the cubing tab and set the active cube
			gui.UpdateTabTo(GuiTabState.CUBING);
			gui.GetTab<GuiCubingTab>()._cubeButton.ChangeItem(item.type);
			// Must be after recalc, otherwise it affects the calculated stack
			item.stack++;
		}

		public abstract IRollingStrategy<RollingStrategyContext> GetRollingStrategy(Item item, RollingStrategyProperties properties);
	}
}
