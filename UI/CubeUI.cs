using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Loot.Core;
using Loot.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.UI
{
	public class CubeUI : UIState
	{
		private UIPanel _backPanel;
		internal UICubeItemPanel _cubePanel;
		internal UIRerollItemPanel _rerollItemPanel;
		internal UIModifierPanel[] _modifierPanels;
		internal UIImageButton _rerollButton;
		private const float padding = 5f;

		public static bool Visible = false;

		internal static void ToggleUI()
		{
			Main.PlaySound(SoundID.MenuOpen);
			Visible = !Visible;
			var ui = Loot.Instance.cubeUI;

			// If ui closed, we need to retrieve the slotted items
			if (!Visible)
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.CloseUI);
				
				// Clear of the slotted cube type
				ui._cubePanel?.item.TurnToAir();

				// If there is an item slotted
				if (ui._rerollItemPanel != null
				    && !ui._rerollItemPanel.item.IsAir)
				{
					Main.LocalPlayer.QuickSpawnClonedItem(ui._rerollItemPanel.item, ui._rerollItemPanel.item.stack);
					ui._rerollItemPanel.item.TurnToAir();
				}

				// Clear lines
				ui.UpdateModifierLines();
			}
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			_backPanel = new UIPanel();
			_backPanel.Width.Set(600f, 0f);
			_backPanel.Height.Set(200f, 0f);

			_backPanel.Left.Set(Main.screenWidth / 2f - _backPanel.Width.Pixels / 2f, 0f);
			_backPanel.Top.Set(Main.screenHeight / 2f - _backPanel.Height.Pixels / 2f, 0f);

			_backPanel.BackgroundColor = new Color(73, 94, 171);

			_backPanel.OnMouseDown += DragStart;
			_backPanel.OnMouseUp += DragEnd;

			base.Append(_backPanel);

			Texture2D buttonDeleteTexture = ModLoader.GetTexture("Terraria/UI/Camera_5");
			UIImageButton closeButton = new UIImageButton(buttonDeleteTexture);
			closeButton.Left.Set(_backPanel.Width.Pixels - buttonDeleteTexture.Width - 20f, 0f);
			closeButton.Top.Set(0f, 0f);
			closeButton.Width.Set(buttonDeleteTexture.Width, 0f);
			closeButton.Height.Set(buttonDeleteTexture.Height, 0f);
			closeButton.OnClick += (evt, element) => { ToggleUI(); };
			_backPanel.Append(closeButton);

			_cubePanel = new UICubeItemPanel(0, 0, Loot.Instance.GetTexture("Core/Cubes/MagicalCube"), "Place a cube here");
			_cubePanel.Top.Set(0f, 0f);
			_backPanel.Append(_cubePanel);

			_rerollItemPanel = new UIRerollItemPanel(0, 0, ModLoader.GetTexture("Terraria/Item_3827"), "Place an item to reroll here");
			_rerollItemPanel.Top.Set(_cubePanel.Top.Pixels + _rerollItemPanel.Height.Pixels + padding, 0f);
			_backPanel.Append(_rerollItemPanel);

			_modifierPanels = new UIModifierPanel[4];
			for (int i = 0; i < 4; i++)
			{
				_modifierPanels[i] = new UIModifierPanel();
				var panel = _modifierPanels[i];

				panel.Left.Set(padding + _cubePanel.Left.Pixels + _cubePanel.Width.Pixels, 0f);
				panel.Height.Set(40, 0f);
				panel.Top.Set(i * padding + panel.Height.Pixels * i, 0f);
				panel.Width.Set(_backPanel.Width.Pixels - closeButton.Width.Pixels * 1.5f - panel.Left.Pixels - padding, 0f);
				panel.BackgroundColor = new Color(73, 94, 171);
				_backPanel.Append(panel);
			}

			Texture2D rerollTexture = ModLoader.GetTexture("Terraria/UI/Craft_Toggle_3");
			_rerollButton = new UIImageButton(rerollTexture);
			_rerollButton.Width.Set(UIItemPanel.panelwidth, 0f);
			_rerollButton.Height.Set(UIItemPanel.panelwidth, 0f);
			_rerollButton.Top.Set(_rerollItemPanel.Top.Pixels + _rerollItemPanel.Height.Pixels + padding, 0f);
			_rerollButton.OnClick += OnRerollClick;
			_backPanel.Append(_rerollButton);
		}

		private void OnRerollClick(UIMouseEvent evt, UIElement listeningelement)
		{
			if (_rerollItemPanel != null
			    && _cubePanel != null
			    && !_rerollItemPanel.item.IsAir && !_cubePanel.item.IsAir
			    && _rerollItemPanel.CanTakeItem(_rerollItemPanel.item)
			    && _cubePanel.CanTakeItem(_cubePanel.item))
			{
				_cubePanel.RecalculateStack();

				// No slotted cubes available
				if (_cubePanel.item.stack <= 0)
				{
					SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
					_cubePanel.item.TurnToAir();
					return;
				}

				// Refresh item
				Item newItem = new Item();
				newItem.netDefaults(_rerollItemPanel.item.type);
				if (_rerollItemPanel.item.prefix > 0)
					newItem.Prefix(_rerollItemPanel.item.prefix);

				// Roll new pool
				// @todo different rolling options
				ModifierContext ctx = new ModifierContext
				{
					Method = ModifierContextMethod.OnCubeReroll,
					Item = newItem,
					Player = Main.LocalPlayer
				};

				var pool = EMMItem.GetItemInfo(newItem).RollNewPool(ctx);
				pool?.ApplyModifiers(newItem);
				_rerollItemPanel.item = newItem.Clone();
				UpdateModifierLines();
				Main.PlaySound(SoundID.Item37, -1, -1);

				// Update slotted cube stacks
				foreach (Item item in Main.LocalPlayer.inventory)
				{
					if (item.type == _cubePanel.item.type)
					{
						item.stack--;
						break;
					}
				}

				_cubePanel.RecalculateStack();

				// No slotted cubes available
				if (_cubePanel.item.stack <= 0)
				{
					_cubePanel.item.TurnToAir();
				}
			}
			else
			{
				SoundHelper.PlayCustomSound(SoundHelper.SoundType.Decline);
			}
		}

		private Vector2 _offset;
		private bool _dragging = false;

		private void DragEnd(UIMouseEvent evt, UIElement listeningelement)
		{
			Vector2 end = evt.MousePosition;
			_dragging = false;

			_backPanel.Left.Set(end.X - _offset.X, 0f);
			_backPanel.Top.Set(end.Y - _offset.Y, 0f);

			Recalculate();
		}

		private void DragStart(UIMouseEvent evt, UIElement listeningelement)
		{
			_offset = new Vector2(evt.MousePosition.X - _backPanel.Left.Pixels, evt.MousePosition.Y - _backPanel.Top.Pixels);
			_dragging = true;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Vector2 mousePosition = new Vector2((float) Main.mouseX, (float) Main.mouseY);
			if (_backPanel.ContainsPoint(mousePosition))
			{
				Main.LocalPlayer.mouseInterface = true;
			}

			if (_dragging)
			{
				_backPanel.Left.Set(mousePosition.X - _offset.X, 0f);
				_backPanel.Top.Set(mousePosition.Y - _offset.Y, 0f);
				Recalculate();
			}
		}

		public void UpdateModifierLines()
		{
			foreach (var panel in _modifierPanels)
			{
				panel.ResetText();
			}

			if (_rerollItemPanel != null
			    && !_rerollItemPanel.item.IsAir
			    && _modifierPanels != null)
			{
				int i = 0;
				foreach (var lines in EMMItem.GetActivePool(_rerollItemPanel.item).Select(x => x.TooltipLines))
				{
					if (i > 4)
						break;

					string line = lines.Aggregate("", (current, tooltipLine) => current + $"{tooltipLine.Text} ");
					line = line.TrimEnd();
					_modifierPanels[i].UpdateText(line);
					i++;
				}
				
				Recalculate();
			}
		}
	}
}