using Loot.Core.System.Modifier;
using Terraria;

namespace Loot.Core.Caching
{
	public sealed partial class ModifierCachePlayer
	{
		private void UpdateVanityCache()
		{
			// vanity
			for (int i = 13; i < 18 + player.extraAccessorySlots; i++)
			{
				var oldEquip = _oldVanityEquips[i - 13];
				var newEquip = player.armor[i];

				// If equip slot needs an update
				if (!_forceEquipUpdate && oldEquip != null && newEquip == oldEquip)
					continue;

				Ready = false;

				// detach old first
				if (oldEquip != null && !oldEquip.IsAir && ActivatedModifierItem.Item(oldEquip).IsCheated)
				{
					foreach (Modifier m in EMMItem.GetActivePool(oldEquip))
					{
						AddDetachItem(oldEquip, m);
					}
				}

				// attach new
				if (newEquip != null && !newEquip.IsAir && ActivatedModifierItem.Item(newEquip).IsCheated)
				{
					foreach (Modifier m in EMMItem.GetActivePool(newEquip))
					{
						AddAttachItem(newEquip, m);
					}
				}

				_oldVanityEquips[i - 13] = newEquip;
			}
		}

		private void UpdateHeldItemCache()
		{
			// If held item needs an update
			if (_oldSelectedItem == player.selectedItem)
				return;

			Ready = false;

			// detach old held item
			Item oldSelectedItem = player.inventory[_oldSelectedItem];
			if (oldSelectedItem != null && !oldSelectedItem.IsAir && IsMouseUsable(oldSelectedItem))
			{
				foreach (Modifier m in EMMItem.GetActivePool(oldSelectedItem))
				{
					AddDetachItem(oldSelectedItem, m);
				}
			}

			// attach new held item
			if (player.HeldItem != null && !player.HeldItem.IsAir && IsMouseUsable(player.HeldItem))
			{
				foreach (Modifier m in EMMItem.GetActivePool(player.HeldItem))
				{
					AddAttachItem(player.HeldItem, m);
				}
			}

			_oldSelectedItem = player.selectedItem;
		}

		private bool UpdateMouseItemCache()
		{
			// If held item needs an update
			if (_oldMouseItem != null && _oldMouseItem == Main.mouseItem)
				return false;

			Ready = false;

			// detach old mouse item
			if (_oldMouseItem != null && !_oldMouseItem.IsAir && IsMouseUsable(_oldMouseItem))
			{
				foreach (Modifier m in EMMItem.GetActivePool(_oldMouseItem))
				{
					AddDetachItem(_oldMouseItem, m);
				}
			}

			// attach new held item
			if (Main.mouseItem != null && !Main.mouseItem.IsAir && IsMouseUsable(Main.mouseItem))
			{
				foreach (Modifier m in EMMItem.GetActivePool(player.HeldItem))
				{
					AddAttachItem(Main.mouseItem, m);
				}
			}

			_oldMouseItem = Main.mouseItem;
			return Main.mouseItem != null && !Main.mouseItem.IsAir;
		}

		private void UpdateEquipsCache()
		{
			for (int i = 0; i < 8 + player.extraAccessorySlots; i++)
			{
				var oldEquip = _oldEquips[i];
				var newEquip = player.armor[i];

				// If equip slot needs an update
				if (!_forceEquipUpdate && oldEquip != null && newEquip == oldEquip)
					continue;

				Ready = false;

				// detach old first
				if (oldEquip != null && !oldEquip.IsAir)
				{
					foreach (Modifier m in EMMItem.GetActivePool(oldEquip))
					{
						AddDetachItem(oldEquip, m);
					}
				}

				// attach new
				if (newEquip != null && !newEquip.IsAir)
				{
					foreach (Modifier m in EMMItem.GetActivePool(newEquip))
					{
						AddAttachItem(newEquip, m);
					}
				}

				_oldEquips[i] = newEquip;
			}
		}
	}
}
