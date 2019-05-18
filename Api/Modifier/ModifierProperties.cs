using System;
using System.IO;
using Loot.Api.Builder;
using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.Api.Modifier
{
	/// <summary>
	/// Defines the properties of a modifier
	/// </summary>
	public class ModifierProperties
	{
		public static ModifierPropertiesBuilder Builder => new ModifierPropertiesBuilder();

		public float MinMagnitude { get; private set; } = 1f;
		public float MaxMagnitude { get; private set; } = 1f;
		public float MagnitudeStrength { get; private set; } = 1f;
		public float BasePower { get; private set; } = 1f;
		public float RarityLevel { get; private set; } = 1f;
		public float RollChance { get; private set; } = 1f;
		public int RoundPrecision { get; private set; } = 0;
		public bool IsUnique { get; private set; } = false;
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

		public float RoundedPower { get; private set; }

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
				{"ModifierPropertiesSaveVersion", 1}
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

		/// <summary>
		/// The ModifierPropertiesBuilder implements the builder pattern for ModifierProperties
		/// It provides a structured way to create ("build") an instance of ModifierProperties
		/// It is used in <see cref="Modifier.GetModifierProperties"/> to dynamically build
		/// the ModifierProperties for that Modifier.
		/// </summary>
		public class ModifierPropertiesBuilder : PropertyBuilder<ModifierProperties>
		{
			protected override ModifierProperties DefaultProperty
			{
				set
				{
					Property.MinMagnitude = value.MinMagnitude;
					Property.MaxMagnitude = value.MaxMagnitude;
					Property.MagnitudeStrength = value.MagnitudeStrength;
					Property.BasePower = value.BasePower;
					Property.RarityLevel = value.RarityLevel;
					Property.RollChance = value.RollChance;
					Property.RoundPrecision = value.RoundPrecision;
					Property.IsUnique = value.IsUnique;
				}
			}

			public ModifierPropertiesBuilder WithMinMagnitude(float val)
			{
				Property.MinMagnitude = val;
				return this;
			}

			public ModifierPropertiesBuilder WithMaxMagnitude(float val)
			{
				Property.MaxMagnitude = val;
				return this;
			}

			public ModifierPropertiesBuilder WithMagnitudeStrength(float val)
			{
				Property.MagnitudeStrength = val;
				return this;
			}

			public ModifierPropertiesBuilder WithBasePower(float val)
			{
				Property.BasePower = val;
				return this;
			}

			public ModifierPropertiesBuilder WithRarityLevel(float val)
			{
				Property.RarityLevel = val;
				return this;
			}

			public ModifierPropertiesBuilder WithRollChance(float val)
			{
				Property.RollChance = val;
				return this;
			}

			public ModifierPropertiesBuilder WithRoundPrecision(int val)
			{
				Property.RoundPrecision = val;
				return this;
			}

			public ModifierPropertiesBuilder IsUniqueModifier(bool val)
			{
				Property.IsUnique = val;
				return this;
			}
		}
	}
}
