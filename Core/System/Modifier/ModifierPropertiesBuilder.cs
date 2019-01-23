namespace Loot.Core.System.Modifier
{
	/// <summary>
	/// The ModifierPropertiesBuilder implements the builder pattern for ModifierProperties
	/// It provides a structured way to create ("build") an instance of ModifierProperties
	/// It is used in <see cref="Modifier.GetModifierProperties"/> to dynamically build
	/// the ModifierProperties for that Modifier.
	/// </summary>
	public class ModifierPropertiesBuilder
	{
		public ModifierPropertiesBuilder()
		{
			DefaultProperties = new ModifierProperties();
		}

		public ModifierPropertiesBuilder(ModifierProperties defaultProperties)
		{
			DefaultProperties = defaultProperties;
		}

		public ModifierPropertiesBuilder(ModifierPropertiesBuilder defaultBuilder)
		{
			DefaultBuilder = defaultBuilder;
		}

		public float MinMagnitude { protected get; set; }

		public float MaxMagnitude { protected get; set; }

		public float MagnitudeStrength { protected get; set; }

		public float BasePower { protected get; set; }

		public float RarityLevel { protected get; set; }

		public float RollChance { protected get; set; }

		public int RoundPrecision { protected get; set; }

		public bool IsUnique { protected get; set; }

		public ModifierProperties DefaultProperties
		{
			set
			{
				MinMagnitude = value.MinMagnitude;
				MaxMagnitude = value.MaxMagnitude;
				MagnitudeStrength = value.MagnitudeStrength;
				BasePower = value.BasePower;
				RarityLevel = value.RarityLevel;
				RollChance = value.RollChance;
				RoundPrecision = value.RoundPrecision;
				IsUnique = value.IsUnique;
			}
		}

		public ModifierPropertiesBuilder DefaultBuilder
		{
			set => DefaultProperties = value.Build();
		}

		public ModifierPropertiesBuilder UsingDefault(ModifierProperties val)
		{
			DefaultProperties = val;
			return this;
		}

		public ModifierPropertiesBuilder UsingDefault(ModifierPropertiesBuilder val)
		{
			DefaultBuilder = val;
			return this;
		}

		public ModifierPropertiesBuilder WithMinMagnitude(float val)
		{
			MinMagnitude = val;
			return this;
		}

		public ModifierPropertiesBuilder WithMaxMagnitude(float val)
		{
			MaxMagnitude = val;
			return this;
		}

		public ModifierPropertiesBuilder WithMagnitudeStrength(float val)
		{
			MagnitudeStrength = val;
			return this;
		}

		public ModifierPropertiesBuilder WithBasePower(float val)
		{
			BasePower = val;
			return this;
		}

		public ModifierPropertiesBuilder WithRarityLevel(float val)
		{
			RarityLevel = val;
			return this;
		}

		public ModifierPropertiesBuilder WithRollChance(float val)
		{
			RollChance = val;
			return this;
		}

		public ModifierPropertiesBuilder WithRoundPrecision(int val)
		{
			RoundPrecision = val;
			return this;
		}

		public ModifierPropertiesBuilder IsUniqueModifier(bool val)
		{
			IsUnique = val;
			return this;
		}

		public ModifierProperties Build()
		{
			return new ModifierProperties(
				MinMagnitude,
				MaxMagnitude,
				MagnitudeStrength,
				BasePower,
				RarityLevel,
				RollChance,
				RoundPrecision,
				IsUnique);
		}
	}
}
