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
