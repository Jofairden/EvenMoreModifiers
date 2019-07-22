using System;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	internal sealed class StaticAssetAttribute : Attribute
	{
		public readonly string AssetPath;

		public StaticAssetAttribute(string _path)
		{
			AssetPath = _path;
		}

		public Texture2D LoadTexture2D()
		{
			string path = AssetPath;
			if (path.StartsWith("Loot/"))
				path = path.Replace("Loot/", "");
			return Loot.Instance.GetTexture(path);
		}
	}
}
