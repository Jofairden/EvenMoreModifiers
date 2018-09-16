using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Loot.UI
{
	public class UIInteractableItemPanel : UIItemPanel
	{
		protected bool RightClickFunctionalityEnabled = true;
		protected bool TakeUserItemOnClick = true;


		public UIInteractableItemPanel(int netID = 0, int stack = 0, Texture2D hintTexture = null, string hintText = null)
			: base(netID, stack, hintTexture, hintText)
		{
			base.OnClick += UIInteractableItemPanel_OnClick;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (RightClickFunctionalityEnabled && base.IsMouseHovering && Main.mouseRight)
			{
				// Slot has an item
				if (!item.IsAir)
				{
					// Open inventory
					Main.playerInventory = true;

					// Mouseitem has to be the same as slot
					if (Main.stackSplit <= 1 &&
						(Main.mouseItem.type == item.type
						 || Main.mouseItem.IsAir))
					{
						int num2 = Main.superFastStack + 1;
						for (int j = 0; j < num2; j++)
						{
							// Mouseitem is air, or stack is smaller than maxstack, and slot has stack
							if (Main.mouseItem.IsAir
								|| (Main.mouseItem.stack < Main.mouseItem.maxStack)
								&& item.stack > 0)
							{
								// Play sound
								if (j == 0)
								{
									Main.PlaySound(18, -1, -1, 1);
								}

								// Mouseitem is air, clone item
								if (Main.mouseItem.IsAir)
								{
									Main.mouseItem = item.Clone();
									// If it has prefix, copy it
									if (item.prefix != 0)
									{
										Main.mouseItem.Prefix((int)item.prefix);
									}

									Main.mouseItem.stack = 0;
								}

								// Add to mouseitem stack
								Main.mouseItem.stack++;
								// Take from slot stack
								item.stack--;
								Main.stackSplit = Main.stackSplit == 0 ? 15 : Main.stackDelay;

								// Reset item
								if (item.stack <= 0)
								{
									item.TurnToAir();
								}
							}
						}
					}
				}

				PostOnRightClick();
			}
		}

		public virtual bool CanTakeItem(Item item) => !item.IsAir;

		public virtual void PostOnRightClick()
		{
		}

		public virtual void PreOnClick(UIMouseEvent evt, UIElement e)
		{
		}

		public virtual void PostOnClick(UIMouseEvent evt, UIElement e)
		{
		}

		private void UIInteractableItemPanel_OnClick(UIMouseEvent evt, UIElement e)
		{
			PreOnClick(evt, e);

			// Slot has an item
			if (!item.IsAir)
			{
				// Only slot has an item
				if (Main.mouseItem.IsAir)
				{
					Main.PlaySound(SoundID.Grab);
					Main.playerInventory = true;
					if (TakeUserItemOnClick)
					{
						Main.mouseItem = item.Clone();
					}

					item.TurnToAir();
				}
				// Mouse has an item
				// Can take mouse item
				else if (CanTakeItem(Main.mouseItem))
				{
					Main.PlaySound(SoundID.Grab);
					Main.playerInventory = true;
					// Items are the same type
					if (item.type == Main.mouseItem.type)
					{
						// Attempt increment stack
						var newStack = item.stack + Main.mouseItem.stack;
						// Mouse item stack fits, increment
						if (item.maxStack >= newStack)
						{
							item.stack = newStack;
							if (TakeUserItemOnClick)
							{
								Main.mouseItem.TurnToAir();
							}
						}
						// Doesn't fit, set item to maxstack, set mouse item stack to difference
						else
						{
							var stackDiff = newStack - item.maxStack;
							item.stack = item.maxStack;
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
						var tmp = item.Clone();
						var tmp2 = Main.mouseItem.Clone();
						if (TakeUserItemOnClick)
						{
							Main.mouseItem = tmp;
						}

						item = tmp2;
					}
				}
			}
			// Slot has no item
			// Slot can take mouse item
			else if (CanTakeItem(Main.mouseItem))
			{
				Main.PlaySound(SoundID.Grab);
				Main.playerInventory = true;
				item = Main.mouseItem.Clone();
				if (TakeUserItemOnClick)
				{
					Main.mouseItem.TurnToAir();
				}
			}

			// PostClick
			PostOnClick(evt, e);
		}
	}
}