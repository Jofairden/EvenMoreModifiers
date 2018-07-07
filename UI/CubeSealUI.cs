using Loot.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI
{
	public sealed class CubeSealUI : DraggableUIState
	{
		private UIPanel _backPanel;
		internal UIImageButton _cubePanel;
		internal UIInteractableItemPanel _itemPanel;
		private const float padding = 5f;

		public override void ToggleUI(UserInterface theInterface, UIState uiStateInstance)
		{
			base.ToggleUI(theInterface, uiStateInstance);
			
			if (!Visible)
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.CloseUI);
				
				if (_itemPanel != null && !_itemPanel.item.IsAir)
				{
					Main.LocalPlayer.QuickSpawnClonedItem(_itemPanel.item);
					_itemPanel.item.TurnToAir();
				}
			}
		}

		public override void OnInitialize()
		{
			// Makes back panel, and assigns it as the drag panel
			_backPanel = new UIPanel();
			_backPanel.Width.Set(600f, 0f);
			_backPanel.Height.Set(200f, 0f);
			_backPanel.Left.Set(Main.screenWidth / 2f - _backPanel.Width.Pixels / 2f, 0f);
			_backPanel.Top.Set(Main.screenHeight / 2f - _backPanel.Height.Pixels / 2f, 0f);
			_backPanel.BackgroundColor = new Color(73, 94, 171);
			base.Append(_backPanel);
			AssignDragPanel(_backPanel);
			base.OnInitialize();

			var sealCubeTexture = Loot.Instance.GetTexture("Core/Cubes/CubeOfSealing");
			_cubePanel = new UIImageButton(sealCubeTexture);
			_cubePanel.Width.Set(sealCubeTexture.Width, 0f);
			_cubePanel.Height.Set(sealCubeTexture.Height, 0f);
			_cubePanel.Left.Set(padding, 0f);
			_backPanel.Append(_cubePanel);

			_itemPanel = new UIInteractableItemPanel(hintTexture: ModLoader.GetTexture("Terraria/Item_3827"), hintText: "Place an item to seal here");
			_itemPanel.Width.Set(UIItemPanel.panelwidth, 0f);
			_itemPanel.Height.Set(UIItemPanel.panelheight, 0f);
			_itemPanel.Left.Set(_cubePanel.Left.Pixels + _cubePanel.Width.Pixels + padding, 0f);
			_backPanel.Append(_itemPanel);
		}
	}
}