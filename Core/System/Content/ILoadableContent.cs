using Terraria.ModLoader;

namespace Loot.Core.System.Core
{
	public interface ILoadableContent
	{
		Mod Mod { get; }
		uint Type { get; }
		string Name { get; }
	}

	// Used to restrict setting these properties
	// only internally.
	// Kind of sucks, but the only way with interfaces
	internal interface ILoadableContentSetter
	{
		Mod Mod { set; }
		uint Type { set; }
	}
}
