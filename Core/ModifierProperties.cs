using System;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Core
{
	/// <summary>
	/// Defines the properties of a modifier
	/// </summary>
	public class ModifierProperties
	{
		public float MinMagnitude { get; private set; }
		public float MaxMagnitude { get; private set; }
		public float MagnitudeStrength { get; private set; }
		public float BasePower { get; private set; }
		public float RarityLevel { get; private set; }
		public float RollChance { get; private set; }
		public int RoundPrecision { get; private set; }
		public float Magnitude { get; private set; }
		private float _power;
		public float Power
		{
			get { return _power; }
			private set
			{
				_power = value;
				RoundedPower = (float)Math.Round(value, RoundPrecision);
			}
		}
		public float RoundedPower
		{
			get;
			private set;
		}
		public bool UniqueModifier { get; private set; }

		public ModifierProperties(float minMagnitude = 1f, float maxMagnitude = 1f, float magnitudeStrength = 1f, float basePower = 1f, float rarityLevel = 1f, float rollChance = 1f, int roundPrecision = 0, bool uniqueRoll = false, bool mergeTooltips = false)
		{
			Set(minMagnitude, maxMagnitude, magnitudeStrength, basePower, rarityLevel, rollChance, roundPrecision);
		}

		public ModifierProperties Set(float? minMagnitude = null, float? maxMagnitude = null, float? magnitudeStrength = null, float? basePower = null, float? rarityLevel = null, float? rollChance = null, int? roundPrecision = null, bool? uniqueRoll = null)
		{
			MinMagnitude = minMagnitude ?? MinMagnitude;
			MaxMagnitude = maxMagnitude ?? MaxMagnitude;
			MagnitudeStrength = magnitudeStrength ?? MagnitudeStrength;
			BasePower = basePower ?? BasePower;
			RarityLevel = rarityLevel ?? RarityLevel;
			RollChance = rollChance ?? RollChance;
			RoundPrecision = roundPrecision ?? RoundPrecision;
			UniqueModifier = uniqueRoll ?? UniqueModifier;
			return this;
		}

		public ModifierProperties RollMagnitudeAndPower(float? magnitude = null, float? power = null, float magnitudePower = 1f)
		{
			/* Roll power TODO support /luck/ stat */
			Magnitude = magnitude ?? RollMagnitude(magnitudePower);
			Power = power ?? RollPower();
			return this;
		}

		private float RollMagnitude(float magnitudePower)
		{
			float randomMag = (MinMagnitude + Main.rand.NextFloat() * (MaxMagnitude - MinMagnitude));
			return randomMag * MagnitudeStrength * magnitudePower;
		}

		private float RollPower()
		{
			return BasePower * Magnitude;
		}

		public virtual void NetReceive(Item item, BinaryReader reader)
		{
		}

		internal static ModifierProperties _NetReceive(Item item, BinaryReader reader)
		{
			var p = new ModifierProperties().RollMagnitudeAndPower(reader.ReadSingle(), reader.ReadSingle());
			p.NetReceive(item, reader);
			return p;
		}

		public virtual void NetSend(Item item, BinaryWriter writer)
		{
		}

		internal static void _NetSend(Item item, ModifierProperties properties, BinaryWriter writer)
		{
			writer.Write(properties.Magnitude);
			writer.Write(properties.Power);
			properties.NetSend(item, writer);
		}

		public virtual void Save(Item item, TagCompound tag)
		{
		}

		internal static TagCompound _Save(Item item, ModifierProperties properties)
		{
			var tc = new TagCompound
			{
				{"Magnitude", properties.Magnitude},
				{"Power", properties.Power},
				{"ModifierPropertiesSaveVersion", 1 }
			};
			properties.Save(item, tc);
			return tc;
		}

		public virtual void Load(Item item, TagCompound tag)
		{
		}

		internal static ModifierProperties _Load(Item item, TagCompound tag)
		{
			ModifierProperties prop;
			try
			{
				prop = new ModifierProperties().RollMagnitudeAndPower(tag.GetFloat("Magnitude"), tag.GetFloat("Power"));
			}
			catch (Exception)
			{
				// Something was wrong with the TC, roll new values
				prop = new ModifierProperties().RollMagnitudeAndPower();
			}
			prop.Load(item, tag);
			return prop;
		}
	}
}