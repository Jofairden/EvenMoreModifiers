using System;

namespace Loot.Api.Attributes
{
	/// <summary>
	/// Used to mark a class to be ignored by EMM loading.
	/// This is used for the Null classes which are singletons
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class DoNotLoadAttribute : Attribute
	{
	}
}
