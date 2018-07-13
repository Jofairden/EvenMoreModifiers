using System;
using System.Collections.Generic;
using System.Reflection;
using Loot.Core;
using Terraria;
using Terraria.ModLoader;

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
					try
					{
						Delegate handler = Delegate.CreateDelegate(evt.EventHandlerType, effect, method);
						evt.AddEventHandler(player, handler);
					}
					catch (Exception e)
					{
						ErrorLogger.Log(e.ToString());
						Main.NewTextMultiline(e.ToString());
						Main.NewText("An error just occurred. Please let the mod author know on the forums and show a screenshot of it", 255, 0, 0);
					}
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
					try
					{
						Delegate handler = Delegate.CreateDelegate(evt.EventHandlerType, effect, method);
						evt.RemoveEventHandler(player, handler);
					}
					catch (Exception e)
					{
						ErrorLogger.Log(e.ToString());
						Main.NewTextMultiline(e.ToString());
						Main.NewText("An error just occurred. Please let the mod author know on the forums and show a screenshot of it.", 255, 0, 0);
					}
				}
			}
		}
	}
}