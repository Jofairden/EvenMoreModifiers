using System;
using Microsoft.Xna.Framework;

namespace Loot.Core.Cubes
{
	/// <summary>
	/// Defines properties that will be used when an item is being rolled
	/// </summary>
	public sealed class ItemRollProperties
	{
		/// <summary>
		/// The minimum amount of modifiers to roll
		/// </summary>
		public int MinModifierRolls { get; set; } = 1;

		private int _maxRollableLines = 4;

		/// <summary>
		/// The maximum amount of modifiers that can roll
		/// </summary>
		public int MaxRollableLines
		{
			get { return _maxRollableLines; }
			set { _maxRollableLines = (int) MathHelper.Clamp(value, 1f, 4f); }
		}

		/// <summary>
		/// Chance to roll a consecutive modifier
		/// </summary>
		public float RollNextChance { get; set; } = 0.5f;

		/// <summary>
		/// The minimum strength of them to roll
		/// </summary>
		public float MinStrength { get; set; } = 1f;

		/// <summary>
		/// Force a specific pool to roll
		/// Note that <see cref="OverrideRollModifierPool"/> will override this property
		/// </summary>
		public ModifierPool ForceModifierPool { get; set; } = null;

		/// <summary>
		/// The chance to roll a pre-defined (weighted) pool
		/// </summary>
		public float RollPredefinedPoolChance { get; set; } = 0.25f;

		/// <summary>
		/// Apply a custom behavior for rolling a pool
		/// </summary>
		public Func<ModifierPool> OverrideRollModifierPool { get; set; } = null;
	}
}
