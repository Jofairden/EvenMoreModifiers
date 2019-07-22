using System.Collections.Generic;
using System.IO;
using System.Linq;
using Loot.Api.Core;
using Loot.Modifiers.Base;
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
		private static readonly (int type, int time, float chance)[] BuffPairs =
		{
			(BuffID.Confused, 100, 1f),
			(BuffID.CursedInferno, 100, 1f),
			(BuffID.Frostburn, 100, 1f),
			(BuffID.OnFire, 100, 1f),
			(BuffID.Poisoned, 100, 1f),
			(BuffID.Ichor, 100, 1f),
		};

		private readonly int _len = BuffPairs.GetLength(0);

		public int GetRolledIndex() => _index;
		private int _index = -1;

		public override void NetReceive(Item item, BinaryReader reader)
		{
			base.NetReceive(item, reader);
			_index = reader.ReadInt32();
			_timeScaleFactor = reader.ReadSingle();
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			base.NetSend(item, writer);
			writer.Write(_index);
			writer.Write(_timeScaleFactor);
		}

		public override void Save(Item item, TagCompound tag)
		{
			base.Save(item, tag);
			tag.Add("_index", _index);
			tag.Add("_timeScaleFactor", _timeScaleFactor);
		}

		public override void Load(Item item, TagCompound tag)
		{
			base.Load(item, tag);
			_index = tag.GetAsInt("_index");
			// Showcase rolling a stat properly that previously wasn't present
			if (tag.ContainsKey("_timeScaleFactor"))
			{
				_timeScaleFactor = tag.GetAsShort("_timeScaleFactor");
				if (_timeScaleFactor <= 0f) RollTimeScaleFactor();
			}
			else
			{
				RollTimeScaleFactor();
			}
		}

		private void RollTimeScaleFactor()
		{
			_timeScaleFactor = (0.5f + Main.rand.NextFloat() * (0.25f)) + Properties.Power / 100f;
		}

		public override void Roll(ModifierContext ctx, IEnumerable<Modifier> rolledModifiers)
		{
			base.Roll(ctx, rolledModifiers);

			// Exclude already rolled random debuffs from the list
			// Try rolling a new random one
			var similarModsBuffTypes = rolledModifiers.Where(x => x is RandomDebuff).Cast<RandomDebuff>().Select(x => x.BuffType);
			var rollableBuffTypes = BuffPairs.Select(x => x.type).Except(similarModsBuffTypes);
			int randBuffIndex = Main.rand.Next(rollableBuffTypes.Count());
			_index = BuffPairs.ToList().FindIndex(x => x.type == rollableBuffTypes.ElementAt(randBuffIndex));
			RollTimeScaleFactor();
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

		private float _timeScaleFactor = 1f;
		public override int BuffType => BuffPairs[_index].type;
		public override int BuffTime => (int) (BuffPairs[_index].time * _timeScaleFactor);
		public override float BuffInflictionChance => BuffPairs[_index].chance;
	}
}
