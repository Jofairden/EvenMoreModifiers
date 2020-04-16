using System;
using System.Collections.Generic;
using Loot.Api.Core;

namespace Loot.Api.Strategy
{
	/// <summary>
	/// Defines properties that will be used when an item is being rolled in a <see cref="IRollingStrategy{T}"/>
	/// These can interact with the rolling behavior, and even override certain things
	/// </summary>
	public sealed class RollingStrategyProperties
	{
		private int _minRollableLines = 1;

		/// <summary>
		/// The minimum amount of modifiers to roll
		/// </summary>
		public int MinRollableLines
		{
			get => _minRollableLines;
			set
			{
				_minRollableLines = value;
				if (value > MaxRollableLines)
					MaxRollableLines = value;
			}
		}

		private int _maxRollableLines = 2;

		/// <summary>
		/// The maximum amount of modifiers that can roll
		/// </summary>
		public int MaxRollableLines
		{
			get => _maxRollableLines;
			set => _maxRollableLines = value;
		}

		/// <summary>
		/// Chance to roll a consecutive modifier
		/// </summary>
		public float RollNextChance { get; set; } = 0.5f;

		/// <summary>
		/// The minimum strength of lines to roll
		/// </summary>
		public float MagnitudePower { get; set; } = 1f;

		/// <summary>
		/// Force a specific pool to roll
		/// Note that <see cref="OverrideRollModifierPool"/> will override this property
		/// </summary>
		public ModifierPool ForceModifierPool { get; set; } = null;

		/// <summary>
		/// The chance to roll a pre-defined (weighted) pool
		/// </summary>
		public float RollPredefinedPoolChance { get; set; } = 0f;

		/// <summary>
		/// Apply a custom behavior for rolling a pool
		/// </summary>
		public Func<ModifierPool> OverrideRollModifierPool { get; set; } = null;

		public ModifierRarity ForceModifierRarity { get; set; } = null;
		public Func<ModifierRarity> OverrideRollModifierRarity { get; set; } = null;

		public Func<ModifierContext, bool> CanUpgradeRarity { get; set; } = ctx => true;
		public Func<ModifierContext, bool> CanDowngradeRarity { get; set; } = ctx => true;

		/// <summary>
		/// Gives extra luck when rolling modifiers
		/// </summary>
		public float ExtraLuck { get; set; } = 0f;

		/// <summary>
		/// Returns a list of lines that will be rolled initially
		/// </summary>
		public Func<List<Modifier>> PresetLines { get; set; }
	}
}
