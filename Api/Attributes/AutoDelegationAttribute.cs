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
			=> names.Select(GetFilteredName);

		private string GetFilteredName(string name)
		{
			if (name.StartsWith("On")) name = name.Substring(2);
			if (!name.EndsWith("Event")) name = $"{name}Event";
			return name;
		}

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
						Loot.Logger.Error("An error just occurred. Please share this error log with the mod author.", e);
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
						Loot.Logger.Error("An error just occurred. Please share this error log with the mod author.", e);
					}
				}
			}
		}

		// TODO I forgot why I added this
		//public bool IsMethodCompatibleWithDelegate<T>(MethodInfo method) where T : class
		//{
		//	Type delegateType = typeof(T);
		//	MethodInfo delegateSignature = delegateType.GetMethod("Invoke");

		//	bool parametersEqual = delegateSignature
		//		.GetParameters()
		//		.Select(x => x.ParameterType)
		//		.SequenceEqual(method.GetParameters()
		//			.Select(x => x.ParameterType));

		//	return delegateSignature.ReturnType == method.ReturnType &&
		//	       parametersEqual;
		//}
	}
}
