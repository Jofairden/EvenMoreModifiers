using System;

namespace Loot.Api.Attributes
{
	/// <summary>
	/// Used to mark a class to be ignored by EMM loading.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class DoNotLoadAttribute : Attribute
	{
	}
}
