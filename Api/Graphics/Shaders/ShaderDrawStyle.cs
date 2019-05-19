using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Loot.Api.Graphics.Shaders
{
	public abstract class ShaderDrawStyle
	{
		public static class Styles
		{
			public static DefaultShaderDrawStyle Default = new DefaultShaderDrawStyle();
			public static AlternateShaderDrawStyle Alternate = new AlternateShaderDrawStyle();
		}

		/// <summary>
		/// Gets the draw offset for the shader
		/// Different styles can be programmed to achieve different looks
		/// </summary>
		/// <param name="i">The i-th segment</param>
		public abstract Vector2 GetDrawOffset(ShaderEntity entity, int i);
	}
}
