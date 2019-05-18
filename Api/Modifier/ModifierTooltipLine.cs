using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loot.Api.Modifier
{
	/// <summary>
	/// Defines a tooltip line of a modifier
	/// A modifier can have multiple lines
	/// </summary>
	public class ModifierTooltipLine
	{
		public string Text;
		public Color? Color;

		internal static ModifierTooltipBuilder Builder => new ModifierTooltipBuilder();

		public class ModifierTooltipBuilder : Builder.PropertyBuilder<List<ModifierTooltipLine>>
		{
			protected override List<ModifierTooltipLine> DefaultProperty
			{
				set
				{
					Property.Clear();
					Property.AddRange(value);
				}
			}

			public ModifierTooltipBuilder WithLines(IEnumerable<ModifierTooltipLine> lines)
			{
				Property.AddRange(lines);
				return this;
			}

			public ModifierTooltipBuilder WithLine(ModifierTooltipLine line)
			{
				Property.Add(line);
				return this;
			}

			public ModifierTooltipBuilder WithLine(string text, Color? color = null)
			{
				Property.Add(new ModifierTooltipLine
				{
					Text = text,
					Color = color
				});
				return this;
			}

			public ModifierTooltipBuilder WithPositive(string text)
			{
				Property.Add(new PositiveTooltipLine(
					text
				));
				return this;
			}

			public ModifierTooltipBuilder WithNegative(string text)
			{
				Property.Add(new NegativeTooltipLine(
					text
				));
				return this;
			}

			public ModifierTooltipBuilder WithPositives(params string[] texts)
			{
				foreach (string text in texts)
				{
					WithPositive(text);
				}

				return this;
			}

			public ModifierTooltipBuilder WithNegatives(params string[] texts)
			{
				foreach (string text in texts)
				{
					WithNegative(text);
				}

				return this;
			}
		}
	}

	public class PositiveTooltipLine : ModifierTooltipLine
	{
		public PositiveTooltipLine(string text = null)
		{
			if (text != null) Text = text;
			Color = Microsoft.Xna.Framework.Color.Lime;
		}
	}

	public class NegativeTooltipLine : ModifierTooltipLine
	{
		public NegativeTooltipLine(string text = null)
		{
			if (text != null) Text = text;
			Color = Microsoft.Xna.Framework.Color.Red;
		}
	}
}
