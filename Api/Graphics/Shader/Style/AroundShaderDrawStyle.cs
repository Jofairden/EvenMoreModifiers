using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Loot.Api.Graphics.Shader.Style
{
	public sealed class AroundShaderDrawStyle : NormalShaderDrawStyle
	{
		public enum OffsetStyle
		{
			/// <summary>
			/// Closely reflects going around the item
			/// </summary>
			NORMAL,
			/// <summary>
			/// Will draw closer to the center
			/// </summary>
			ALTERNATE
		}

		public OffsetStyle offsetStyle;

		public AroundShaderDrawStyle(OffsetStyle offsetStyle = OffsetStyle.NORMAL, ShaderDrawStyleProperties properties = null) : base(properties)
		{
			this.offsetStyle = offsetStyle;
		}

		/// <summary>
		/// Gets the draw offset for the shader
		/// Different styles can be programmed to achieve different looks
		/// </summary>
		/// <param name="i">The i-th segment</param>
		public Vector2 GetDrawOffset(int i)
		{
			if (offsetStyle == OffsetStyle.NORMAL)
			{
				return new Vector2(0, Properties.DrawDistance).RotatedBy((float)i / Properties.NumSegments * MathHelper.TwoPi);
			}
			if (offsetStyle == OffsetStyle.ALTERNATE)
			{
				var halfDist = Properties.DrawDistance / 2;
				var offY = halfDist + halfDist * i % halfDist;
				return new Vector2(0, offY).RotatedBy((float)i / Properties.NumSegments * MathHelper.TwoPi);
			}
			return Vector2.Zero;
		}

		public override void DrawShader(SpriteBatch spriteBatch, ShaderEntity entity)
		{
			var centerPos = entity.DrawData.position;
			for (int i = 0; i < Properties.NumSegments; i++)
			{
				entity.DrawData.position = centerPos + GetDrawOffset(i);
				base.DrawShader(spriteBatch, entity);
			}
		}
	}
}
