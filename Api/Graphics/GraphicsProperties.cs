using Loot.Api.Builder;

namespace Loot.Api.Graphics
{
	public class GraphicsProperties
	{
		public static GraphicsPropertiesBuilder Builder => new GraphicsPropertiesBuilder();

		public bool SkipUpdatingDrawData { get; set; }
		public bool SkipDrawing { get; set; }

		public class GraphicsPropertiesBuilder : PropertyBuilder<GraphicsProperties>
		{
			protected override GraphicsProperties DefaultProperty
			{
				set
				{
					Property.SkipUpdatingDrawData = value.SkipUpdatingDrawData;
					Property.SkipDrawing = value.SkipDrawing;
				}
			}

			public GraphicsPropertiesBuilder SkipDrawing(bool value)
			{
				Property.SkipDrawing = value;
				return this;
			}
		}
	}
}
