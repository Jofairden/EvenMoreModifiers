using Loot.Api.Builder;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Loot.Api.Graphics.Shader.Style
{
	public abstract class ShaderDrawStyle
	{
		public ShaderDrawStyleProperties Properties;

		protected ShaderDrawStyle(ShaderDrawStyleProperties properties = null)
		{
			Properties = properties ?? ShaderDrawStyleProperties.Builder.Build();
		}

		public abstract void DrawShader(SpriteBatch spriteBatch, ShaderEntity entity);

		public class ShaderDrawStyleProperties
		{
			public static ShaderDrawStylePropertiesBuilder Builder => new ShaderDrawStylePropertiesBuilder();

			public int NumSegments { get; set; } = 8;
			public int DrawDistance { get; set; } = 4;

			public class ShaderDrawStylePropertiesBuilder : PropertyBuilder<ShaderDrawStyleProperties>
			{
				protected override ShaderDrawStyleProperties DefaultProperty
				{
					set
					{
						Property.NumSegments = value.NumSegments;
						Property.DrawDistance = value.DrawDistance;
					}
				}

				public ShaderDrawStylePropertiesBuilder WithSegments(int num)
				{
					Property.NumSegments = num;
					return this;
				}

				public ShaderDrawStylePropertiesBuilder WithDrawDistance(int dist)
				{
					Property.DrawDistance = dist;
					return this;
				}
			}
		}
	}
}
