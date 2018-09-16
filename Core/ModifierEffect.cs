using Loot.Core.Attributes;
using System;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace Loot.Core
{
	/// <summary>
	/// A ModifierEffect signifies the effect of a modifier on a player
	/// It should house the implementation, and delegation of such effect
	/// Methods on effects can be delegated from ModPlayer
	/// </summary>
	public abstract class ModifierEffect : ICloneable
	{
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }
		public virtual string Name => GetType().Name;

		/// <summary>
		/// Keeps track of if this particular modifier is being delegated or not
		/// This is used to check if this effect's automatic delegation needs to be performed
		/// </summary>
		public bool IsBeingDelegated { get; internal set; }

		public ModifierEffect AsNewInstance()
			=> (ModifierEffect)Activator.CreateInstance(GetType());

		/// <summary>
		/// Called when the ModPlayer initializes the effect
		/// </summary>
		public virtual void OnInitialize(ModifierPlayer player)
		{
		}

		/// <summary>
		/// Automatically called when the ModPlayer does its ResetEffects
		/// Also automatically called when the delegations of the effect detach
		/// </summary>
		public virtual void ResetEffects(ModifierPlayer player)
		{
		}

		/// <summary>
		/// Allows modders to perform various delegations when this modifier is detected to become active
		/// This method will be invoked one time when the modifier becomes 'active'
		/// Alternatively automatic delegation can be used, using the AutoDelegation attribute
		/// </summary>
		public virtual void AttachDelegations(ModifierPlayer player)
		{
		}

		internal void _DetachDelegations(ModifierPlayer player)
		{
			// We need to reset effects first, as they will no longer run
			// after the effect is detached

			// Regular reset
			ResetEffects(player);

			// This part is actually kind of unneeded anymore
			// But im keeping it in in case someone, for some reason,
			// wants to split their ResetEffects in multiple methods
			// without each other calling them.
			// Performance is no issue since automatic attach and detach
			// is handled by ModifierCachePlayer

			// Look for ResetEffects and manually invoke it
			var resetEffects = GetType()
				.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(x => x.GetCustomAttributes().OfType<AutoDelegation>().Any())
				.ToDictionary(x => x, y => y.GetCustomAttribute<AutoDelegation>());

			foreach (var kvp in resetEffects.Where(x => x.Value.DelegationTypes.Contains("OnResetEffects")))
			{
				try
				{
					kvp.Key.Invoke(this, new object[] { player.player });
				}
				catch (Exception e)
				{
				}
			}

			// Now detach
			DetachDelegations(player);
		}

		/// <summary>
		/// Allows modders to undo their performed delegations in <see cref="AttachDelegations"/>
		/// </summary>
		public virtual void DetachDelegations(ModifierPlayer player)
		{
		}

		/// <summary>
		/// A modder can provide custom cloning of effects in this hook
		/// </summary>
		public virtual void Clone(ref ModifierEffect clone)
		{
		}

		public object Clone()
		{
			ModifierEffect clone = (ModifierEffect)MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			Clone(ref clone);
			return clone;
		}

		/// <summary>
		/// Returns the ModifierEffect specified by type, null if not present
		/// </summary>
		public static ModifierEffect GetModifierEffect(ushort type)
			=> EMMLoader.GetModifierEffect(type);
	}
}
