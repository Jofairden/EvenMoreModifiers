using System;
using System.Linq;
using System.Reflection;
using Loot.Core;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	/// <summary>
	/// The following code caches the item held and items equipped by players
	/// When equips and held items change, they respective modifiers are automatically
	/// called to detach and attach.
	/// </summary>
	// ReSharper disable once ClassNeverInstantiated.Global
	public sealed class ModifierCachePlayer : ModPlayer
	{
		private bool IsMouseUsable(Item item) => item.damage > 0;
		
		private Item _oldHeldItem;
		private Item[] _oldEquips;
		private bool _forceEquipUpdate;
		internal bool Ready;

		public override void Initialize()
		{
			_oldHeldItem = null;
			_oldEquips = new Item[8];
			_forceEquipUpdate = false;
			Ready = false;
		}

		private void AutoDetachDelegations(Player player, Modifier modifier)
		{
			var methods = modifier
				.GetType()
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(x => x.GetCustomAttributes(typeof(AutoDelegation), false).Length > 0)
				.ToArray();
			foreach (MethodInfo method in methods)
			{
				var attr = method.GetCustomAttribute<AutoDelegation>();
				attr.Detach(ModifierPlayer.Player(player), method, modifier);
			}
		}

		private void AutoAttachDelegations(Player player, Modifier modifier)
		{
			var methods = modifier
				.GetType()
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(x => x.GetCustomAttributes(typeof(AutoDelegation), false).Length > 0)
				.ToArray();
			foreach (MethodInfo method in methods)
			{
				var attr = method.GetCustomAttribute<AutoDelegation>();
				attr.Attach(ModifierPlayer.Player(player), method, modifier);
			}
		}

		public override void PreUpdate()
		{
			// If held item needs an update
			if (_oldHeldItem == null || _oldHeldItem.IsNotTheSameAs(player.HeldItem))
			{
				try
				{
					Ready = false;

					// detach old held item
					if (_oldHeldItem != null && !_oldHeldItem.IsAir)
					{
						foreach (Modifier m in EMMItem.GetActivePool(_oldHeldItem))
						{
							AutoDetachDelegations(player, m);
							m._DetachDelegations(_oldHeldItem, ModifierPlayer.Player(player));
						}
					}

					if (player.HeldItem != null && !player.HeldItem.IsAir && IsMouseUsable(player.HeldItem))
					{
						// attach new held item
						foreach (Modifier m in EMMItem.GetActivePool(player.HeldItem))
						{
							AutoAttachDelegations(player, m);
							m.AttachDelegations(player.HeldItem, ModifierPlayer.Player(player));
						}
					}

					_oldHeldItem = player.HeldItem;
				}
				catch (Exception e)
				{
					Main.NewTextMultiline(e.ToString());
				}
			}

			// If our equips cache is not the right size we resize it and force an update
			if (_oldEquips.Length != 8 + player.extraAccessorySlots)
			{
				Ready = false;
				_forceEquipUpdate = true;
				Array.Resize(ref _oldEquips, 8 + player.extraAccessorySlots);
			}

			for (int i = 0; i < 8 + player.extraAccessorySlots; i++)
			{
				try
				{
					var oldEquip = _oldEquips[i];
					var newEquip = player.armor[i];

					// If equip slot needs an update
					if (_forceEquipUpdate || oldEquip == null || newEquip.IsNotTheSameAs(oldEquip))
					{
						Ready = false;

						// detach old first
						if (oldEquip != null && !oldEquip.IsAir)
						{
							foreach (Modifier m in EMMItem.GetActivePool(oldEquip))
							{
								AutoDetachDelegations(player, m);
								m._DetachDelegations(oldEquip, ModifierPlayer.Player(player));
							}
						}

						if (newEquip != null && !newEquip.IsAir)
						{
							// attach new
							foreach (Modifier m in EMMItem.GetActivePool(newEquip))
							{
								AutoAttachDelegations(player, m);
								m.AttachDelegations(newEquip, ModifierPlayer.Player(player));
							}
						}

						_oldEquips[i] = newEquip;
					}
				}
				catch (Exception e)
				{
					Main.NewTextMultiline(e.ToString());
				}
			}

			_forceEquipUpdate = false;
			Ready = true;
		}
	}
}