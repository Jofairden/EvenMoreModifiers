using System.Linq;
using Loot.Core.System.Core;
using Loot.Core.System.Loaders;
using Terraria.Utilities;

namespace Loot.Core.System.Content
{
	public class ModifierPoolContent : BaseContent<ModifierPool>
	{
		/// <summary>
		/// Returns a random weighted pool from all available pools that can apply
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
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

		//@todo this might need a better place.
		/// <summary>
		/// Returns the rarity of a pool
		/// </summary>
		/// <param name="modifierPool"></param>
		/// <returns></returns>
		public ModifierRarity GetPoolRarity(ModifierPool modifierPool)
		{
			return (ModifierRarity)ContentLoader.ModifierRarity.Content
				.Select(r => r.Value)
				.OrderByDescending(r => r.RequiredRarityLevel)
				.FirstOrDefault(r => r.MatchesRequirements(modifierPool))
				?.Clone();
		}
	}
}
