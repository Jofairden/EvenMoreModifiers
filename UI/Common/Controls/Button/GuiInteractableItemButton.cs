using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Loot.UI.Common.Controls.Button
{
	internal class GuiInteractableItemButton : GuiItemButton
	{
		protected bool RightClickFunctionalityEnabled = true;
		protected bool TakeUserItemOnClick = true;
		public Action<Item> OnItemChange;

		public bool PerformRegularClickInteraction { get; protected internal set; } = true;

		internal GuiInteractableItemButton(ButtonType buttonType, int netId = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null, string hintOnHover = null) : base(buttonType, netId, stack, hintTexture, hintText, hintOnHover)
		{
			OnClick += HandleClickAction;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (!RightClickFunctionalityEnabled || !IsMouseHovering || !Main.mouseRight)
			{
				return;
			}

			// Slot has an item
			if (!Item.IsAir)
			{
				// Open inventory
				Main.playerInventory = true;

				// Mouseitem has to be the same as slot
				if (Main.stackSplit <= 1 &&
					(Main.mouseItem.type == Item.type
					 || Main.mouseItem.IsAir))
				{
					int num2 = Main.superFastStack + 1;
					for (int j = 0; j < num2; j++)
					{
						// Mouseitem is air, or stack is smaller than maxstack, and slot has stack
						if (!Main.mouseItem.IsAir && ((Main.mouseItem.stack >= Main.mouseItem.maxStack) || Item.stack <= 0))
						{
							continue;
						}

						// Play sound
						if (j == 0)
						{
							Main.PlaySound(18, -1, -1, 1);
						}

						// Mouseitem is air, clone item
						if (Main.mouseItem.IsAir)
						{
							Main.mouseItem = Item.Clone();
							// If it has prefix, copy it
							if (Item.prefix != 0)
							{
								Main.mouseItem.Prefix((int)Item.prefix);
							}

							Main.mouseItem.stack = 0;
						}

						// Add to mouseitem stack
						Main.mouseItem.stack++;
						// Take from slot stack
						Item.stack--;
						Main.stackSplit = Main.stackSplit == 0 ? 15 : Main.stackDelay;

						// Reset item
						if (Item.stack <= 0)
						{
							ChangeItem(0);
						}
					}
				}
			}

			PostOnRightClick();
		}

		public virtual void ChangeItem(int type, Item item = null)
		{
			if (item != null)
			{
				Item = item;
			}
			else if (type == 0)
			{
				Item.TurnToAir();
			}
			else
			{
				Item.SetDefaults(type);
			}

			OnItemChange?.Invoke(Item);
		}

		public virtual bool CanTakeItem(Item givenItem) => !givenItem.IsAir;

		public virtual void PostOnRightClick()
		{
		}

		public virtual void PreOnClick(UIMouseEvent evt, UIElement e)
		{
		}

		public virtual void PostOnClick(UIMouseEvent evt, UIElement e)
		{
		}

		private void HandleClickAction(UIMouseEvent evt, UIElement e)
		{
			PreOnClick(evt, e);

			if (PerformRegularClickInteraction)
			{
				// Slot has an item
				if (!Item.IsAir)
				{
					// Only slot has an item
					if (Main.mouseItem.IsAir)
					{
						Main.PlaySound(SoundID.Grab);
						Main.playerInventory = true;
						if (TakeUserItemOnClick)
						{
							Main.mouseItem = Item.Clone();
						}

						ChangeItem(0);
					}
					// Mouse has an item
					// Can take mouse item
					else if (CanTakeItem(Main.mouseItem))
					{
						Main.PlaySound(SoundID.Grab);
						Main.playerInventory = true;
						// Items are the same type
						if (Item.type == Main.mouseItem.type)
						{
							// Attempt increment stack
							var newStack = Item.stack + Main.mouseItem.stack;
							// Mouse item stack fits, increment
							if (Item.maxStack >= newStack)
							{
								Item.stack = newStack;
								if (TakeUserItemOnClick)
								{
									Main.mouseItem.TurnToAir();
								}
							}
							// Doesn't fit, set item to maxstack, set mouse item stack to difference
							else
							{
								var stackDiff = newStack - Item.maxStack;
								Item.stack = Item.maxStack;
								if (TakeUserItemOnClick)
								{
									Main.mouseItem.stack = stackDiff;
								}
							}
						}
						// Items are not the same type
						else
						{
							// Swap mouse item and slot item
							var tmp = Item.Clone();
							var tmp2 = Main.mouseItem.Clone();
							if (TakeUserItemOnClick)
							{
								Main.mouseItem = tmp;
							}

							ChangeItem(0, tmp2);
						}
					}
				}
				// Slot has no item
				// Slot can take mouse item
				else if (CanTakeItem(Main.mouseItem))
				{
					Main.PlaySound(SoundID.Grab);
					Main.playerInventory = true;
					ChangeItem(0, Main.mouseItem.Clone());
					if (TakeUserItemOnClick)
					{
						Main.mouseItem.TurnToAir();
					}
				}
			}

			PostOnClick(evt, e);
		}
	}
}
