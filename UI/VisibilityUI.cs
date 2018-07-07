using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Loot.UI
{
	/// <summary>
	/// A UIState which primarily goal is to provide easy visibility toggling
	/// </summary>
	public abstract class VisibilityUI : UIState
	{
		public bool Visible;

		public virtual void ToggleUI(UserInterface theInterface, UIState uiStateInstance)
		{
			// If new state toggled but old visibility state present, that one needs to be toggled first
			if (theInterface.CurrentState is VisibilityUI
			    && theInterface.CurrentState != uiStateInstance)
			{
				(theInterface.CurrentState as VisibilityUI).ToggleUI(theInterface, theInterface.CurrentState);
			}
			
			// Toggle the state
			Visible = !Visible;
			theInterface.ResetLasts();
			theInterface.SetState(Visible ? uiStateInstance : null);
			
			Main.PlaySound(SoundID.MenuOpen);
		}
	}
}