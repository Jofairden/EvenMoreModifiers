using Loot.Api.Builder;
using Loot.Api.Graphics.Glowmask;
using Loot.Api.Graphics.Shader.Style;

namespace Loot.Api.Graphics.Shader
{
	/// <summary>
	/// Defines the <see cref="GraphicsProperties"/> for <see cref="ShaderEntity"/>s which derives from <see cref="GlowmaskGraphicsProperties"/>
	/// </summary>
	public class ShaderGraphicsProperties : GlowmaskGraphicsProperties
	{
		public new static ShaderGraphicsPropertiesBuilder Builder => new ShaderGraphicsPropertiesBuilder();

		public ShaderDrawLayer DrawLayer { get; set; } = ShaderDrawLayer.Back;
		public ShaderDrawStyle ShaderDrawStyle { get; set; } = new NormalShaderDrawStyle();
		public bool SkipDrawingSubject { get; set; }
		public bool SkipDrawingShader { get; set; }

		/// <summary>
		/// A builder for <see cref="ShaderGraphicsProperties"/>
		/// </summary>
		public class ShaderGraphicsPropertiesBuilder : PropertyBuilder<ShaderGraphicsProperties>
		{
			protected override ShaderGraphicsProperties DefaultProperty
			{
				set
				{
					Property.SkipDrawing = value.SkipDrawing;
					Property.SkipDrawingSubject = value.SkipDrawingSubject;
					Property.SkipDrawingGlowmask = value.SkipDrawingGlowmask;
					Property.SkipDrawingShader = value.SkipDrawingShader;
				}
			}

			public ShaderGraphicsPropertiesBuilder WithDrawLayer(ShaderDrawLayer drawLayer)
			{
				Property.DrawLayer = drawLayer;
				return this;
			}

			public ShaderGraphicsPropertiesBuilder WithShaderDrawStyle(ShaderDrawStyle shaderDrawStyle)
			{
				Property.ShaderDrawStyle = shaderDrawStyle;
				return this;
			}

			public ShaderGraphicsPropertiesBuilder SkipDrawingSubject(bool value)
			{
				Property.SkipDrawingSubject = value;
				return this;
			}

			public ShaderGraphicsPropertiesBuilder SkipDrawingShader(bool value)
			{
				Property.SkipDrawingShader = value;
				return this;
			}

			public ShaderGraphicsPropertiesBuilder SkipDrawing(bool value)
			{
				Property.SkipDrawing = value;
				return this;
			}

			public ShaderGraphicsPropertiesBuilder SkipDrawingGlowmask(bool val)
			{
				Property.SkipDrawingGlowmask = val;
				return this;
			}
		}
	}
}
