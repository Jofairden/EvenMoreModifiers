using Loot.Core.System.Modifier;
using Microsoft.Xna.Framework;
using System;

namespace Loot.Core.Cubes
{
	/// <summary>
	/// Defines properties that will be used when an item is being rolled
	/// These can interact with the rolling behavior, and even override certain things
	/// </summary>
	public sealed class ItemRollProperties
	{
		private int _minModifierRolls = 1;

		/// <summary>
		/// The minimum amount of modifiers to roll
		/// </summary>
		public int MinModifierRolls
		{
			get => _minModifierRolls;
			set => _minModifierRolls = (int)MathHelper.Max(value, 1);
		}

		private int _maxRollableLines = 4;

		/// <summary>
		/// The maximum amount of modifiers that can roll
		/// </summary>
		public int MaxRollableLines
		{
			get => _maxRollableLines;
			set => _maxRollableLines = (int)MathHelper.Clamp(value, 1f, 4f);
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
	}
}
