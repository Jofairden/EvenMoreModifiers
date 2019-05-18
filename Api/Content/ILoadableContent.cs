using Terraria.ModLoader;

namespace Loot.Api.Content
{
	public interface ILoadableContent
	{
		Mod Mod { get; }
		uint Type { get; }
		string Name { get; }
	}
}
