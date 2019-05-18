using Terraria.ModLoader;

namespace Loot.Api.Content
{
	/// <summary>
	/// Used to restrict setting properties of <see cref="ILoadableContent"/> to internal only
	/// </summary>
	internal interface ILoadableContentSetter
	{
		Mod Mod { set; }
		uint Type { set; }
	}
}
