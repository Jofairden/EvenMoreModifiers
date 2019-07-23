using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loot.Api.Attributes;
using Loot.Api.Core;
using Loot.Api.Delegators;
using Loot.Api.Ext;
using Loot.Hacks;
using Loot.ModSupport;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Caching
{
	/*
	 * Do note that this class is very important
	 * And without this class, any of the delegations will not work at all
	 * This class automatically delegates only the applicable modifiers and their effects
	 * \\WHEN NEEDED\\. This means only 'active' items are being updated, that means any held item
	 * and items that are equipped (armor/accessory)
	 */

	/// <summary>
	/// Caches the item held and items equipped by players.
	/// When equips and held items change, their respective modifiers' effects are automatically called to detach and attach their delegations.
	/// </summary>
	public sealed partial class ModifierCachePlayer : ModPlayer
	{
		private static bool IsMouseUsable(Item item) => item.damage > 0;

		internal bool Ready;

		private int _oldSelectedItem;
		private Item _oldMouseItem;
		private Item[] _oldEquips;
		private Item[] _oldVanityEquips;
		private bool _forceEquipUpdate;
		private Item[] _oldCheatSheetEquips;
		private List<Type> _modifierEffects;
		private List<(Item, Modifier)> _detachList;
		private List<(Item, Modifier)> _attachList;

		public override void Initialize()
		{
			_oldSelectedItem = player.selectedItem;
			_oldMouseItem = null;
			_oldEquips = new Item[8];
			_oldVanityEquips = new Item[8];
			_forceEquipUpdate = false;
			_oldCheatSheetEquips = new Item[6]; // MaxExtraAccessories = 6
			_modifierEffects = new List<Type>();
			_detachList = new List<(Item, Modifier)>();
			_attachList = new List<(Item, Modifier)>();
			Ready = false;
		}

		/// <summary>
		/// This method will return a list of <see cref="ModifierEffect"/>s based on <see cref="Modifier"/>s passed to it 
		/// </summary>
		private IEnumerable<ModifierEffect> GetModifierEffectsForDelegations(IEnumerable<(Item item, Modifier modifier)> list, ModifierDelegatorPlayer modDelegatorPlayer, Func<ModifierEffect, bool> conditionFunc)
		{
			var tempList = new List<ModifierEffect>();
			foreach (var delegationTuple in list)
			{
				var effectsAttribute =
					delegationTuple.item
						.GetType()
						.GetCustomAttribute<UsesEffectAttribute>();

				if (effectsAttribute == null) continue;
				tempList.AddRange(effectsAttribute.Effects
					.Select(modDelegatorPlayer.GetEffect)
					.Where(modEffect => modEffect != null && conditionFunc.Invoke(modEffect)));
			}

			return tempList;
		}

		/// <summary>
		/// This method handles updating all delegations
		/// First, it will perform 'manual' detachments calling <see cref="ModifierEffect.DetachDelegations(ModifierDelegatorPlayer)"/>
		/// then it will perform 'manual' attachments calling <see cref="ModifierEffect.AttachDelegations(ModifierDelegatorPlayer)"/>
		/// These will unbind/bind <see cref="ModifierEffect.ResetEffects"/> to <see cref="ModifierDelegatorPlayer.OnResetEffects"/>
		/// and set <see cref="ModifierEffect.IsBeingDelegated"/> to false or true.
		/// 
		/// After this, the automatic detachments/attachments are done which will bind any methods
		/// to whichever method noted in their <see cref="AutoDelegation"/> attribute.
		/// </summary>
		private void UpdateAttachments()
		{
			ModifierDelegatorPlayer modDelegatorPlayer = ModifierDelegatorPlayer.GetPlayer(player);

			// Manual detach
			var detachEffects = GetModifierEffectsForDelegations(_detachList, modDelegatorPlayer, (e) => e.IsBeingDelegated && !_modifierEffects.Contains(e.GetType()));
			var attachEffects = GetModifierEffectsForDelegations(_attachList, modDelegatorPlayer, (e) => !e.IsBeingDelegated);
			// Automatic delegation lists
			var orderedDetachList = OrderDelegationList(_detachList, modDelegatorPlayer)
				.Where(x => x.effect.IsBeingDelegated && !_modifierEffects.Contains(x.effect.GetType()))
				.GroupBy(x => x.methodInfo)
				.Select(x => x.First())
				.ToList();

			var orderedAttachList = OrderDelegationList(_attachList, modDelegatorPlayer)
				.Where(x => !x.effect.IsBeingDelegated)
				.GroupBy(x => x.methodInfo)
				.Select(x => x.First())
				.ToList();

			// Manual detach
			foreach (var effect in detachEffects.Distinct())
			{
				modDelegatorPlayer.ResetEffectsEvent -= effect.ResetEffects;
				effect._DetachDelegations(modDelegatorPlayer);
				effect.IsBeingDelegated = false;
			}

			// Manual attach
			foreach (var effect in attachEffects.Distinct())
			{
				modDelegatorPlayer.ResetEffectsEvent += effect.ResetEffects;
				effect.AttachDelegations(modDelegatorPlayer);
				effect.IsBeingDelegated = true;
			}

			// Auto delegation detach
			foreach (var (methodInfo, effect) in orderedDetachList)
			{
				var attr = methodInfo.GetCustomAttribute<AutoDelegation>();
				attr.Detach(modDelegatorPlayer, methodInfo, effect);
			}

			// Auto delegation attach
			foreach (var (methodInfo, effect) in orderedAttachList)
			{
				var attr = methodInfo.GetCustomAttribute<AutoDelegation>();
				attr.Attach(modDelegatorPlayer, methodInfo, effect);
			}
		}

		/// <summary>
		/// This method orders delegation entries following the rules of <see cref="DelegationPrioritizationAttribute"/>
		/// which orders by Early/Late prioritization
		/// and a level ranging from 0-999 which indicates how much that prioritization should be enforced.
		/// </summary>
		private IEnumerable<(MethodInfo methodInfo, ModifierEffect effect)> OrderDelegationList(IEnumerable<(Item item, Modifier modifier)> list, ModifierDelegatorPlayer modDelegatorPlayer)
		{
			var delegationEntries = new List<(MethodInfo, ModifierEffect)>();
			var effects = GetModifierEffectsForDelegations(list, modDelegatorPlayer, e => true);

			foreach (var modEffect in effects)
			{
				(MethodInfo, ModifierEffect) MakeEntry((MethodInfo, DelegationPrioritizationAttribute) tuple)
					=> (tuple.Item1, modEffect);

				var delegatedMethods = modEffect
					.GetType()
					.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
					.Where(x => x.GetCustomAttributes(typeof(AutoDelegation), false).Length > 0)
					.Select(x => (methodInfo: x, attribute: (DelegationPrioritizationAttribute) x.GetCustomAttribute(typeof(DelegationPrioritizationAttribute))))
					.ToList();

				delegationEntries.AddRange(
					delegatedMethods
						.Where(x => x.attribute?.DelegationPrioritization == DelegationPrioritization.Early)
						.OrderByDescending(x => x.attribute.DelegationLevel)
						.Select(MakeEntry));

				delegationEntries.AddRange(
					delegatedMethods
						.Where(x => x.attribute == null)
						.Select(MakeEntry));

				delegationEntries.AddRange(
					delegatedMethods
						.Where(x => x.attribute?.DelegationPrioritization == DelegationPrioritization.Late)
						.OrderByDescending(x => x.attribute.DelegationLevel)
						.Select(MakeEntry));
			}

			return delegationEntries;
		}

		public override void PostUpdate()
		{
			// If our equips cache is not the right size we resize it and force an update
			if (_oldEquips.Length != 8 + player.extraAccessorySlots)
			{
				Ready = false;
				_forceEquipUpdate = true;
				Array.Resize(ref _oldEquips, 8 + player.extraAccessorySlots);
			}

			if (_oldVanityEquips.Length != 8 + player.extraAccessorySlots)
			{
				Ready = false;
				_forceEquipUpdate = true;
				Array.Resize(ref _oldVanityEquips, 8 + player.extraAccessorySlots);
			}

			_detachList.Clear();
			_attachList.Clear();

			UpdateEquipsCache();
			if (ModSupportTunneler.GetModSupport<CheatSheetSupport>().ModIsLoaded)
			{
				UpdateCheatSheetCache();
			}

			UpdateVanityCache();

			if (!UpdateMouseItemCache())
			{
				UpdateHeldItemCache();
			}

			CacheModifierEffects();
			UpdateAttachments();

			_forceEquipUpdate = false;
			Ready = true;
		}

		private void AddDetachItem(Item item, Modifier modifier)
		{
			LootModItem.GetInfo(item).IsActivated = false;
			_detachList.Add((item, modifier));
		}

		private void AddAttachItem(Item item, Modifier modifier)
		{
			LootModItem.GetInfo(item).IsActivated = true;
			_attachList.Add((item, modifier));
		}

		private void CacheModifierEffects()
		{
			if (!_forceEquipUpdate && Ready)
				return;

			_modifierEffects.Clear();

			for (int i = 0; i < 8 + player.extraAccessorySlots; i++)
			{
				var equip = player.armor[i];
				if (equip != null && !equip.IsAir)
				{
					CacheItemModifierEffects(equip);
				}
			}

			// vanity
			for (int k = 13; k < 18 + player.extraAccessorySlots; k++)
			{
				var equip = player.armor[k];
				if (equip != null
				    && !equip.IsAir
				    && CheatedItemHackGlobalItem.GetInfo(equip).IsCheated)
				{
					CacheItemModifierEffects(equip);
				}
			}

			if (Main.mouseItem != null && !Main.mouseItem.IsAir && Main.mouseItem.IsWeapon())
			{
				CacheItemModifierEffects(Main.mouseItem);
			}
			else if (player.HeldItem != null && !player.HeldItem.IsAir && player.HeldItem.IsWeapon())
			{
				CacheItemModifierEffects(player.HeldItem);
			}
		}

		private void CacheItemModifierEffects(Item item)
		{
			var mods = LootModItem.GetActivePool(item);
			foreach (var modifier in mods)
			{
				var effectsAttribute = modifier
					.GetType()
					.GetCustomAttribute<UsesEffectAttribute>();

				if (effectsAttribute == null)
					continue;

				ModifierDelegatorPlayer modDelegatorPlayer = ModifierDelegatorPlayer.GetPlayer(player);
				foreach (Type effect in effectsAttribute.Effects)
				{
					var modEffect = modDelegatorPlayer.GetEffect(effect);
					if (modEffect != null && !_modifierEffects.Contains(modEffect.GetType()))
					{
						_modifierEffects.Add(modEffect.GetType());
					}
				}
			}
		}
	}
}
