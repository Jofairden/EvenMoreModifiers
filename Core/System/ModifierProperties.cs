using System;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Core.System
{
	/// <summary>
	/// Defines the properties of a modifier
	/// </summary>
	public class ModifierProperties
	{
		public static ModifierPropertiesBuilder Builder
			=> new ModifierPropertiesBuilder();

		public readonly float MinMagnitude;
		public readonly float MaxMagnitude;
		public readonly float MagnitudeStrength;
		public readonly float BasePower;
		public readonly float RarityLevel;
		public readonly float RollChance;
		public readonly int RoundPrecision;
		public readonly bool IsUnique;
		public float Magnitude { get; internal set; }
		private float _power;
		public float Power
		{
			get => _power;
			internal set
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

		public ModifierProperties(float minMagnitude = 1f, float maxMagnitude = 1f, float magnitudeStrength = 1f, float basePower = 1f, float rarityLevel = 1f, float rollChance = 1f, int roundPrecision = 0, bool isUnique = false)
		{
			MinMagnitude = minMagnitude;
			MaxMagnitude = maxMagnitude;
			MagnitudeStrength = magnitudeStrength;
			BasePower = basePower;
			RarityLevel = rarityLevel;
			RollChance = rollChance;
			RoundPrecision = roundPrecision;
			IsUnique = isUnique;
		}

		public ModifierProperties RollMagnitudeAndPower(float magnitudePower = 1f, float lukStat = 0f)
		{
			Magnitude = RollMagnitude(magnitudePower, lukStat);

			int iterations = (int)Math.Ceiling(lukStat) / 2;

			// makes you more lucky rolling better magnitudes
			for (int i = 0; i < iterations; i++)
			{
				float rolledMagnitude = RollMagnitude(magnitudePower, lukStat);
				if (rolledMagnitude > Magnitude)
				{
					Magnitude = rolledMagnitude;
				}
			}

			Power = RollPower();

			return this;
		}

		private float RollMagnitude(float magnitudePower, float lukStat)
		{
			float useMin = MinMagnitude * (1f + magnitudePower / 10f);
			float useMax = MaxMagnitude * magnitudePower;
			float randomMag = (useMin + Main.rand.NextFloat() * (useMax - useMin));
			return randomMag * MagnitudeStrength;
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
			var p = new ModifierProperties
			{
				Magnitude = reader.ReadSingle(),
				Power = reader.ReadSingle()
			};
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
				prop = new ModifierProperties
				{
					Magnitude = tag.GetFloat("Magnitude"),
					Power = tag.GetFloat("Power")
				};
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
