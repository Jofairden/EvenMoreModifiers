using System;
using System.Collections.Generic;
using System.Reflection;
using Loot.Core;

namespace Loot.Modifiers
{
	/// <summary>
	/// This attribute may be used to skip usage of AttachDelegations and DetachDelegations
	/// Which is a cumbersome side effect of using events
	/// The attribute will automate the process of attaching the method to and detaching the method from
	/// the specified event(s)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class AutoDelegation : Attribute
	{
		private readonly IEnumerable<string> _delegationTypes;

		public IEnumerable<string> DelegationTypes => new List<string>(_delegationTypes);

		public AutoDelegation(params string[] types)
		{
			_delegationTypes = new List<string>(types);
		}

		public void Attach(ModifierPlayer player, MethodInfo method, ModifierEffect effect)
		{
			foreach (string type in _delegationTypes)
			{
				EventInfo evt = player.GetType().GetEvent(type, BindingFlags.Instance | BindingFlags.Public);
				if (evt != null)
				{				
					Delegate handler = Delegate.CreateDelegate(evt.EventHandlerType, effect, method);
					evt.AddEventHandler(player, handler);
				}
			}
		}
		
		public void Detach(ModifierPlayer player, MethodInfo method, ModifierEffect effect)
		{
			foreach (string type in _delegationTypes)
			{
				EventInfo evt = player.GetType().GetEvent(type, BindingFlags.Instance | BindingFlags.Public);
				if (evt != null)
				{				
					Delegate handler = Delegate.CreateDelegate(evt.EventHandlerType, effect, method);
					evt.RemoveEventHandler(player, handler);
				}
			}
		}
	}
}