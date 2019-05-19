using Microsoft.Xna.Framework;
using Terraria;

namespace Loot.Api.Graphics.Shaders
{
	public sealed class DefaultShaderDrawStyle : ShaderDrawStyle
	{
		public override Vector2 GetDrawOffset(ShaderEntity entity, int i)
		{
			return new Vector2(0, entity.DrawDistance).RotatedBy((float)i / entity.NumSegments * MathHelper.TwoPi);
		}
	}
}