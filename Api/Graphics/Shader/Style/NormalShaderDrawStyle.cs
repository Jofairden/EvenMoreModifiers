using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Loot.Api.Graphics.Shader.Style
{
	public class NormalShaderDrawStyle : ShaderDrawStyle
	{
		public NormalShaderDrawStyle(ShaderDrawStyleProperties properties = null) : base(properties)
		{
		}

		public override void DrawShader(SpriteBatch spriteBatch, ShaderEntity entity)
		{
			entity.DrawEntity(spriteBatch);
		}
	}
}
