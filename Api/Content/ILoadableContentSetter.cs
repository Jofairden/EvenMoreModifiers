using Terraria.ModLoader;

namespace Loot.Api.Content
{
	/// <summary>
	/// Used to restrict setting properties of <see cref="ILoadableContent"/> to internal only
	/// This is done by marking the interface itself as internal
	/// This exposes the property setters only internally
	/// This way the properties are still exposed outside the internal assembly but cannot be changed there.
	/// </summary>
	internal interface ILoadableContentSetter
	{
		Mod Mod { set; }
		uint Type { set; }
	}
}
