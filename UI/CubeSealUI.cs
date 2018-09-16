using Loot.Core.Cubes;
using Loot.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI
{
	public sealed class CubeSealUI : CubeUI
	{
		private UIPanel _backPanel;
		private int _cubeCount;
		private UIImageButton _cubePanel;
		internal UISealItemPanel _itemPanel;
		private const float padding = 5f;

		public override bool IsItemValidForUISlot(Item item)
		{
			return _itemPanel != null && _itemPanel.CanTakeItem(item);
		}

		public override bool IsSlottedItemInCubeUI()
		{
			return _itemPanel != null && !_itemPanel.item.IsAir && EMMItem.GetItemInfo(_itemPanel.item).SlottedInCubeUI;
		}

		public override Item SlottedItem => _itemPanel.item;

		internal void RecalculateCubeCount()
		{
			_cubeCount = Main.LocalPlayer.inventory.Where(x => x.modItem is CubeOfSealing).Sum(x => x.stack);
			if (_cubeCount > 999)
			{
				_cubeCount = 999;
			}
		}

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
			else
			{
				RecalculateCubeCount();
			}
		}

		public override void OnInitialize()
		{
			// Makes back panel, and assigns it as the drag panel
			_backPanel = new UIPanel();
			_backPanel.Width.Set(135f, 0f);
			_backPanel.Height.Set(70f, 0f);
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
			_cubePanel.Top.Set(padding, 0f);
			_cubePanel.OnClick += TrySealingSlottedItem;
			_backPanel.Append(_cubePanel);

			_itemPanel = new UISealItemPanel(hintTexture: ModLoader.GetTexture("Terraria/Item_24"), hintText: "Place an item to seal here");
			_itemPanel.Width.Set(UIItemPanel.panelwidth, 0f);
			_itemPanel.Height.Set(UIItemPanel.panelheight, 0f);
			_itemPanel.Left.Set(_cubePanel.Left.Pixels + _cubePanel.Width.Pixels + padding, 0f);
			_backPanel.Append(_itemPanel);

			Texture2D btnCloseTexture = ModLoader.GetTexture("Terraria/UI/InfoIcon_8");
			UIImageButton closeButton = new UIImageButton(btnCloseTexture);
			closeButton.Left.Set(_backPanel.Width.Pixels - btnCloseTexture.Width - 20f, 0f);
			closeButton.Top.Set(0f, 0f);
			closeButton.Width.Set(btnCloseTexture.Width, 0f);
			closeButton.Height.Set(btnCloseTexture.Height, 0f);
			closeButton.OnClick += (evt, element) => { ToggleUI(Loot.Instance.CubeInterface, Loot.Instance.CubeSealUI); };
			_backPanel.Append(closeButton);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (_cubePanel.IsMouseHovering)
			{
				Main.hoverItemName = "Click to seal slotted item's modifiers" +
									 "\nIf an item is already sealed the seal will be removed";
			}
		}

		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			base.DrawChildren(spriteBatch);

			var cubeDimensions = _cubePanel.GetInnerDimensions();
			Utils.DrawBorderStringFourWay(
				spriteBatch,
				Main.fontItemStack,
				_cubeCount.ToString(),
				cubeDimensions.Position().X + 10f,
				cubeDimensions.Position().Y + 26f,
				Color.White,
				Color.Black,
				Vector2.Zero,
				0.8f);
		}

		private void TrySealingSlottedItem(UIMouseEvent evt, UIElement listeningelement)
		{
			if (_cubeCount <= 0
				|| _itemPanel.item.IsAir
				|| !_itemPanel.item.IsModifierRollableItem()
				|| !EMMItem.GetActivePool(_itemPanel.item).Any())
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
				return;
			}

			var cube = Main.LocalPlayer.inventory.FirstOrDefault(x => x.modItem is CubeOfSealing);
			if (cube != null)
			{
				cube.stack--;
				if (cube.stack <= 0)
				{
					cube.TurnToAir();
				}

				RecalculateCubeCount();

				var info = EMMItem.GetItemInfo(_itemPanel.item);
				info.SealedModifiers = !info.SealedModifiers;
				SoundHelper.PlayCustomSound(info.SealedModifiers ? SoundHelper.SoundType.GainSeal : SoundHelper.SoundType.LoseSeal);
			}
		}
	}
}