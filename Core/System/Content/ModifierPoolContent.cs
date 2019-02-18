using Loot.Core.System.Core;
using Loot.Core.System.Modifier;
using System.Linq;
using Terraria.Utilities;

namespace Loot.Core.System.Content
{
	/// <summary>
	/// This class holds all loaded <see cref="ModifierPool"/> content
	/// </summary>
	public class ModifierPoolContent : BaseContent<ModifierPool>
	{
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
			foreach (var m in Content.Where(x => x.Value._CanRoll(ctx)))
			{
				wr.Add(m.Value, m.Value.RollChance);
			}

			var mod = wr.Get();
			return (ModifierPool)mod?.Clone();
		}
	}
}
