using System;
using System.Linq;
using System.Reflection;
using Loot.Core;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Modifiers
{
	/*
	 * Do note that this class is very important
	 * And without this class, any of the delegations will not work at all
	 * This class automatically delegates only the applicable modifiers and their effects
	 * \\WHEN NEEDED\\. This means only 'active' items are being updated, that means any held item
	 * and items that are equipped (armor/accessory)
	 */
	
	/// <summary>
	/// The following code caches the item held and items equipped by players
	/// When equips and held items change, their respective modifiers' effects are automatically
	/// called to detach and attach their delegations
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
		
		// Automatically binds the delegations for a player and given modifier
		// Will look for the UsesEffect attribute on the Modifier, which links
		// that modifier to any effects. If there are effects in present,
		// they will be iterated and an attempt to get those effects
		// off the ModifierPlayer will be attempted. If that succeeds,
		// those effects' methods having the AutoDelegation attribute are
		// searched. For those methods the action is invoked, which will either
		// attach or detach that particular delegation.
		private void AutoBindDelegations(Player player, Modifier modifier, Action<ModifierPlayer, MethodInfo[], ModifierEffect> action)
		{
			var effectsAttribute =
				modifier
					.GetType()
					.GetCustomAttribute<UsesEffect>();

			if (effectsAttribute != null)
			{
				ModifierPlayer modPlayer = ModifierPlayer.Player(player);
				foreach (Type effect in effectsAttribute.Effects)
				{
					var modEffect = modPlayer.GetEffect(effect);
					if (modEffect != null)
					{
						var methods = modEffect
							.GetType()
							.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
							.Where(x => x.GetCustomAttributes(typeof(AutoDelegation), false).Length > 0)
							.ToArray();

						action.Invoke(modPlayer, methods, modEffect);
					}
				}
			}
		}

		private void AutoDetach(Item item, Player player, Modifier modifier)
		{
			AutoBindDelegations(player, modifier, (modplr, methods, effect) =>
			{
				if (effect.IsBeingDelegated)
				{
					modplr.OnResetEffects -= effect.ResetEffects;
					foreach (MethodInfo method in methods)
					{
						var attr = method.GetCustomAttribute<AutoDelegation>();
						attr.Detach(modplr, method, effect);
					}
					effect._DetachDelegations(item, modplr);
					effect.IsBeingDelegated = false;
				}
				ActivatedModifierItem.Item(item).IsActivated = false;
			});
		}
		
		private void AutoAttach(Item item, Player player, Modifier modifier)
		{
			AutoBindDelegations(player, modifier, (modplr, methods, effect) =>
			{
				if (!effect.IsBeingDelegated)
				{
					modplr.OnResetEffects += effect.ResetEffects;
					foreach (MethodInfo method in methods)
					{
						var attr = method.GetCustomAttribute<AutoDelegation>();
						attr.Attach(modplr, method, effect);
					}
					effect.AttachDelegations(item, modplr);
					effect.IsBeingDelegated = true;
				}
				ActivatedModifierItem.Item(item).IsActivated = true;
			});
		}

		public override void PreUpdate()
		{
			// If held item needs an update
			if (_oldHeldItem == null || _oldHeldItem.IsNotTheSameAs(player.HeldItem))
			{
				Ready = false;

				// detach old held item
				if (_oldHeldItem != null && !_oldHeldItem.IsAir && IsMouseUsable(_oldHeldItem)) 
				{
					foreach (Modifier m in EMMItem.GetActivePool(_oldHeldItem))
					{
						AutoDetach(_oldHeldItem, player, m);
					}
				}

				if (player.HeldItem != null && !player.HeldItem.IsAir && IsMouseUsable(player.HeldItem))
				{
					// attach new held item
					foreach (Modifier m in EMMItem.GetActivePool(player.HeldItem))
					{
						AutoAttach(player.HeldItem, player, m);
					}
				}

				_oldHeldItem = player.HeldItem;
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
							AutoDetach(oldEquip, player, m);
						}
					}

					if (newEquip != null && !newEquip.IsAir)
					{
						// attach new
						foreach (Modifier m in EMMItem.GetActivePool(newEquip))
						{
							AutoAttach(newEquip, player, m);
						}
					}

					_oldEquips[i] = newEquip;
				}
			}

			_forceEquipUpdate = false;
			Ready = true;
		}
	}
}