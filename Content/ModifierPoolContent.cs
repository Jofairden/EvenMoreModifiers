using System.Collections.Generic;
using System.Linq;
using Loot.Api.Content;
using Loot.Api.Core;
using Loot.Api.Ext;
using Loot.Pools;

namespace Loot.Content
{
	/// <summary>
	/// This class holds all loaded <see cref="ModifierPool"/> content
	/// </summary>
	public sealed class ModifierPoolContent : LoadableContentBase<ModifierPool>
	{
		public ModifierPool NullPool => Content[0];
		public ModifierPool AllPool => Loot.Instance.GetModifierPool<AllModifiersPool>();
		internal IEnumerable<ModifierPool> Pools => Content.Select(x => x.Value);

		internal override void Load()
		{
			AddContent(NullModifierPool.INSTANCE, Loot.Instance);
		}

		internal override bool CheckContentPiece(ModifierPool contentPiece)
		{
			contentPiece.CacheAttributes();
			return true;
		}
	}
}
