using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loot.Api.Delegators;
using Loot.Api.Modifier;
using Terraria;

namespace Loot.Api.Attributes
{
	/// <inheritdoc cref="Attribute"/>
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
			_delegationTypes = new List<string>(GetTargetNames(types));
		}

		public AutoDelegation(params DelegationTarget[] targets)
		{
			_delegationTypes = new List<string>(
				GetTargetNames(targets
					.Select(target => Enum.GetName(typeof(DelegationTarget), target))));
		}

		private IEnumerable<string> GetTargetNames(IEnumerable<string> names)
			=> names.Select(name => !name.StartsWith("On") ? $"On{name}" : name);

		public void Attach(ModifierDelegatorPlayer delegatorPlayer, MethodInfo method, ModifierEffect effect)
		{
			foreach (string type in _delegationTypes)
			{
				EventInfo evt = delegatorPlayer.GetType().GetEvent(type, BindingFlags.Instance | BindingFlags.Public);
				if (evt != null)
				{
					try
					{
						Delegate handler = Delegate.CreateDelegate(evt.EventHandlerType, effect, method);
						evt.AddEventHandler(delegatorPlayer, handler);
					}
					catch (Exception e)
					{
						Loot.Logger.Error(e);
						Main.NewTextMultiline(e.ToString());
						Main.NewText("An error just occurred. Please let the mod author know on the forums and show a screenshot of it", 255, 0, 0);
					}
				}
			}
		}

		public void Detach(ModifierDelegatorPlayer delegatorPlayer, MethodInfo method, ModifierEffect effect)
		{
			foreach (string type in _delegationTypes)
			{
				EventInfo evt = delegatorPlayer.GetType().GetEvent(type, BindingFlags.Instance | BindingFlags.Public);
				if (evt != null)
				{
					try
					{
						Delegate handler = Delegate.CreateDelegate(evt.EventHandlerType, effect, method);
						evt.RemoveEventHandler(delegatorPlayer, handler);
					}
					catch (Exception e)
					{
						Loot.Logger.Error(e);
						Main.NewTextMultiline(e.ToString());
						Main.NewText("An error just occurred. Please let the mod author know on the forums and show a screenshot of it.", 255, 0, 0);
					}
				}
			}
		}
	}
}
