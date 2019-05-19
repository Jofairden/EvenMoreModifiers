using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Api.Graphics.Shaders
{
	public sealed class AlternateShaderDrawStyle : ShaderDrawStyle
	{
		public override Vector2 GetDrawOffset(ShaderEntity entity, int i)
		{
			var halfDist = entity.DrawDistance / 2;
			var offY = halfDist + halfDist * i % halfDist;
			return new Vector2(0, offY).RotatedBy((float)i / entity.NumSegments * MathHelper.TwoPi);
		}
	}
}