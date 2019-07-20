using Microsoft.Xna.Framework.Graphics;

namespace Loot.Api.Graphics.Shader.Style
{
	/// <summary>
	/// Defines a normal shader draw style
	/// </summary>
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
