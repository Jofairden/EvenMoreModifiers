using Terraria;
using Terraria.DataStructures;

namespace Loot.Ext
{
	internal static class FramingUtils
	{
		public static bool IsTopLeftFrame(this Tile tile) =>
			tile.frameX == 0
			&& tile.frameY == 0;

		public static Point16 GetTopLeftFrame(this Tile tile, int i, int j, int size = 16, int padding = 2) =>
			new Point16(
				i - tile.frameX / (size + padding),
				j - tile.frameY / (size + padding));
	}
}
