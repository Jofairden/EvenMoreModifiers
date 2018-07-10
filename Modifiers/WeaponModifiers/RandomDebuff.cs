using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loot.Core;
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

		public override void Roll(ModifierContext ctx, IEnumerable<Modifier> rolledModifiers)
		{
			base.Roll(ctx, rolledModifiers);

			// Exclude already rolled random debuffs from the list
			// Try rolling a new random one
			var similarModsBuffTypes = rolledModifiers.Where(x => x is RandomDebuff).Cast<RandomDebuff>().Select(x => x.BuffType);
			var rollableBuffTypes = BuffPairs.Select(x => x.BuffType).Except(similarModsBuffTypes);
			int randBuffIndex = Main.rand.Next(rollableBuffTypes.Count());
			_index = BuffPairs.ToList().FindIndex(x => x.BuffType == rollableBuffTypes.ElementAt(randBuffIndex));
		}

		public override bool PostRoll(ModifierContext ctx, IEnumerable<Modifier> rolledModifiers)
		{
			// The roll is only valid, if none other rolled random debuff has the same index
			return base.PostRoll(ctx, rolledModifiers)
			       && _index != -1
			       && rolledModifiers
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
	}
}