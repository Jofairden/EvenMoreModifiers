using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loot.System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers.WeaponModifiers
{
	internal struct DebuffTrigger
	{
		public int BuffType;
		public int BuffTime;
		public float InflictionChance;
	}

	/// <summary>
	/// Rolls a random debuff
	/// </summary>
	public sealed class RandomDebuff : WeaponDebuffModifier
	{
		// TODO , this could should be added to to include min/max magnitude rolls, that influence the time and chance to apply 
		//internal static int[,] BuffPairs =
		//{
		//	{ BuffID.Confused, 120 },
		//	{ BuffID.CursedInferno, 180 },
		//	{ BuffID.Frostburn, 240 },
		//	{ BuffID.OnFire, 300 },
		//	{ BuffID.Poisoned, 480 },
		//	{ BuffID.Ichor, 180 },
		//};

		internal static DebuffTrigger[] BuffPairs =
		{
			new DebuffTrigger {BuffType = BuffID.Confused, BuffTime = 120, InflictionChance = 1f},
			new DebuffTrigger {BuffType = BuffID.CursedInferno, BuffTime = 180, InflictionChance = 1f},
			new DebuffTrigger {BuffType = BuffID.Frostburn, BuffTime = 240, InflictionChance = 1f},
			new DebuffTrigger {BuffType = BuffID.OnFire, BuffTime = 300, InflictionChance = 1f},
			new DebuffTrigger {BuffType = BuffID.Poisoned, BuffTime = 480, InflictionChance = 1f},
			new DebuffTrigger {BuffType = BuffID.Ichor, BuffTime = 180, InflictionChance = 1f}
		};

		private readonly int _len = BuffPairs.GetLength(0);

		public int GetRolledIndex() => _index;
		private int _index = -1;

		public override void NetReceive(Item item, BinaryReader reader)
		{
			base.NetReceive(item, reader);
			_index = reader.ReadInt32();
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			base.NetSend(item, writer);
			writer.Write(_index);
		}

		public override void Save(Item item, TagCompound tag)
		{
			base.Save(item, tag);
			tag.Add("_index", _index);
		}

		public override void Load(Item item, TagCompound tag)
		{
			base.Load(item, tag);
			_index = tag.GetAsInt("_index");
		}

		public override void Roll(ModifierContext ctx)
		{
			base.Roll(ctx);
			_index = Main.rand.Next(_len);
		}

		public override bool PostRoll(ModifierContext ctx, IEnumerable<Modifier> rolledModifiers)
		{
			return rolledModifiers
				.Select(x => x as RandomDebuff)
				.All(x => x?.GetRolledIndex() != _index);
		}

		public override int BuffType => BuffPairs[_index].BuffType;
		public override int BuffTime => BuffPairs[_index].BuffTime;
		public override float BuffInflictionChance => BuffPairs[_index].InflictionChance;

		public override ModifierProperties GetModifierProperties(Item item)
		{
			return base.GetModifierProperties(item).Set(rollChance: _len);
		}

		//public override bool UniqueRoll(ModifierContext ctx)
		//	=> false;

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
