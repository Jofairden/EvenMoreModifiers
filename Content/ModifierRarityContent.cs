using Loot.Api.Content;
using Loot.Api.Core;

namespace Loot.Content
{
	/// <summary>
	/// This class holds all loaded <see cref="ModifierRarity"/> content
	/// </summary>
	public sealed class ModifierRarityContent : LoadableContentBase<ModifierRarity>
	{
		internal override void Load()
		{
			AddContent(NullModifierRarity.INSTANCE, Loot.Instance);
		}
	}
}
