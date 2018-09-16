using System;
using System.Collections.Generic;

namespace Loot.Core.Attributes
{
	/// <summary>
	/// This attribute is used to attach a certain Modifier to
	/// given ModifierEffects (can be attached to more than 1)
	/// This is used for detecting when effects need to become active
	/// (e.g. Light modifier is attached to the LightEffect, which makes
	/// LightEffect become active when an item with a +light modifier is active)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public sealed class UsesEffectAttribute : Attribute
	{
		private readonly IList<Type> _effects;

		public IList<Type> Effects => new List<Type>(_effects);

		public UsesEffectAttribute(params Type[] types)
		{
			// Get a list of effects, try adding them
			_effects = new List<Type>(types);
		}
	}
}
