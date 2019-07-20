using Loot.Api.Content;
using Loot.Api.Modifier;

namespace Loot.Content
{
	/// <summary>
	/// This class holds all loaded <see cref="ModifierRarity"/> content
	/// </summary>
	public sealed class ModifierRarityContent : LoadableContentBase<ModifierRarity>
	{
		internal override void Load()
		{
			AddContent(typeof(NullModifierRarity), Loot.Instance);
		}
	}
}
