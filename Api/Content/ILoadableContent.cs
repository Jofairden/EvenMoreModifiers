using Terraria.ModLoader;

namespace Loot.Api.Content
{
	/// <summary>
	/// Defines a piece of loadable content
	/// The interface is used in classes where the deriving class already derives an existing class
	/// Because classes can only derive from one class, we use interfaces
	/// The interface is used it conjunction with <see cref="ILoadableContentSetter"/>
	/// </summary>
	public interface ILoadableContent
	{
		Mod Mod { get; }
		uint Type { get; }
		string Name { get; }
	}
}
