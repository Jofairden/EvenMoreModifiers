namespace Loot.Core.Graphics
{
	public enum ShaderDrawLayer
	{
		/// <summary>
		/// Will draw the shader entity behind other layers
		/// </summary>
		Back,
		
		/// <summary>
		/// Will draw the shader entity above the regular layer
		/// but below the glowmask layer
		/// </summary>
		Middle,
		
		/// <summary>
		/// Will draw the shader entity above all layers
		/// </summary>
		Front
	}
}
