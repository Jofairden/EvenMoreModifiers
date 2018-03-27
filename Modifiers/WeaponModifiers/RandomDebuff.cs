using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.WeaponModifiers
{
	/// <summary>
	/// Rolls a random debuff
	/// </summary>
	public sealed class RandomDebuff : WeaponDebuffModifier
	{
		// TODO , this could should be added to to include min/max magnitude rolls, that influence the time and chance to apply 
		internal static int[,] BuffPairs = 
		{
			{ BuffID.Confused, 120, },
			{ BuffID.CursedInferno, 180 },
			{ BuffID.Frostburn, 240 },
			{ BuffID.Ichor, 180 },
			{ BuffID.OnFire, 300 },
			{ BuffID.Poisoned, 480 },
		};

		private readonly int _len = BuffPairs.GetLength(0);
		private int _index = -1;

		public override void Save(TagCompound tag)
		{
			tag.Add("_index", _index);
		}

		public override void Load(TagCompound tag)
		{
			_index = tag.GetAsInt("_index");
		}

		public override void Apply(Item item)
		{
			// If don't have an index stored, roll a new one
			if (_index == -1)
				_index = Main.rand.Next(_len);
		}

		public override int BuffType => BuffPairs[_index, 0];
		public override int BuffTime => BuffPairs[_index, 1];
		public override float RollChance => base.RollChance * _len;

		// TODO we need ModPlayer hooks here

		//public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
		//{
		//	base.OnHitNPC(item, player, target, damage, knockBack, crit);

		//	if (Main.rand.NextFloat() <= Power)
		//		target.AddBuff(BuffType, BuffTime);
		//}

		//public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
		//{
		//	base.OnHitPvp(item, player, target, damage, crit);

		//	if (Main.rand.NextFloat() <= Power)
		//		target.AddBuff(BuffType, BuffTime);
		//}
	}
}
