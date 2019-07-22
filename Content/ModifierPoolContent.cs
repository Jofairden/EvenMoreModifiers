using System.Linq;
using Loot.Api.Content;
using Loot.Api.Core;
using Terraria.Utilities;

namespace Loot.Content
{
	/// <summary>
	/// This class holds all loaded <see cref="ModifierPool"/> content
	/// </summary>
	public sealed class ModifierPoolContent : LoadableContentBase<ModifierPool>
	{
		internal override void Load()
		{
			AddContent(NullModifierPool.INSTANCE, Loot.Instance);
		}

		internal override bool CheckContentPiece(ModifierPool contentPiece)
		{
			contentPiece.CacheAttributes();
			return true;
		}

		/// <summary>
		/// Returns a random weighted pool from all available pools that can apply
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns><see cref="ModifierPool"/></returns>
		public ModifierPool GetWeightedPool(ModifierContext ctx)
		{
			var wr = new WeightedRandom<ModifierPool>();
			foreach (var m in Content.Where(x => x.Value._CanRoll(ctx) && x.Value.Type > 0))
			{
				wr.Add(m.Value, m.Value.RollChance);
			}

			var mod = wr.Get();
			return (ModifierPool)mod?.Clone();
		}
	}
}
