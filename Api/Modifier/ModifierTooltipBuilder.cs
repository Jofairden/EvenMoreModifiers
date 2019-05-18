using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loot.Api.Modifier
{
	public class ModifierTooltipBuilder
	{
		private readonly List<ModifierTooltipLine> _lines = new List<ModifierTooltipLine>();

		public ModifierTooltipBuilder WithLines(IEnumerable<ModifierTooltipLine> lines)
		{
			_lines.AddRange(lines);
			return this;
		}

		public ModifierTooltipBuilder WithLine(ModifierTooltipLine line)
		{
			_lines.Add(line);
			return this;
		}

		public ModifierTooltipBuilder WithLine(string text, Color? color = null)
		{
			_lines.Add(new ModifierTooltipLine
			{
				Text = text,
				Color = color
			});
			return this;
		}

		public ModifierTooltipBuilder WithPositive(string text)
		{
			_lines.Add(new PositiveTooltipLine(
				text
			));
			return this;
		}

		public ModifierTooltipBuilder WithNegative(string text)
		{
			_lines.Add(new NegativeTooltipLine(
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

		public ModifierTooltipLine[] Build()
		{
			return _lines.ToArray();
		}
	}
}
