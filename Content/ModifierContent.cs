using Loot.Api.Content;
using Loot.Api.Core;

namespace Loot.Content
{
	/// <summary>
	/// This class holds all loaded <see cref="Modifier"/> content
	/// </summary>
	public sealed class ModifierContent : LoadableContentBase<Modifier>
	{
		internal override void Load()
		{
			AddContent(NullModifier.INSTANCE, Loot.Instance);
		}
	}
}
