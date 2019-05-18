using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Loot.Api.Graphics
{
	/// <summary>
	/// Defines a set of utility methods for graphic subjects
	/// </summary>
	public static class GraphicsUtils
	{
		public static void BeginShaderBatch(this SpriteBatch batch)
		{
			batch.End();
			RasterizerState rasterizerState = Main.LocalPlayer.gravDir == 1f ? RasterizerState.CullCounterClockwise : RasterizerState.CullClockwise;
			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public static void ResetBatch(this SpriteBatch batch)
		{
			batch.End();
			RasterizerState rasterizerState = Main.LocalPlayer.gravDir == 1f ? RasterizerState.CullCounterClockwise : RasterizerState.CullClockwise;
			batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
			//Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		}
	}
}
