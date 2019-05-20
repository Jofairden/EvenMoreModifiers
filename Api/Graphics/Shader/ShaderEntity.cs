using Loot.Api.Ext;
using Loot.Api.Graphics.Glowmask;
using Loot.Api.Graphics.Shader.Style;
using Loot.Api.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Loot.Api.Graphics.Shader
{
	/// <summary>
	/// Defines a Shader entity, part of <see cref="GraphicsEntity{T}"/>
	/// The entity defines a particular 'shader' "of" the entity and can be drawn appropriately
	/// The entity makes use of <see cref="GraphicsModContent"/> to get art assets
	/// </summary>
	public class ShaderEntity : GraphicsEntity<ShaderGraphicsProperties>
	{
		public int ShaderId { get; set; }

		public Texture2D SubjectTexture { get; protected set; }
		public Texture2D ShaderTexture { get; protected set; }

		public ShaderEntity(object subjectIdentity, int shaderId,
			Color? shaderDrawColor = null, short order = 0, ShaderGraphicsProperties props = null)
			: base(subjectIdentity)
		{
			ShaderId = shaderId;
			DrawColor = shaderDrawColor ?? Color.White;
			NeedsUpdate = false;
			Order = order;
			Properties = props ?? ShaderGraphicsProperties.Builder.Build();
		}

		protected override void LoadAssets(Item item)
		{
			var graphicsContent = Loot.ModContentManager.GetContent<GraphicsModContent>();
			graphicsContent?.Prepare(item);

			if (SubjectTexture == null)
			{
				SubjectTexture = Main.itemTexture[item.type];
			}

			if (ShaderTexture == null)
			{
				ShaderTexture = graphicsContent?.GetPreparedShader(item.type.ToString()) ?? SubjectTexture;
			}
		}

		/// <summary>
		/// Will draw all the layers (subject, glowmask, shader) in the proper order 
		/// </summary>
		public void DoDrawLayeredEntity(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float scale, float rotation, GlowmaskEntity glowmaskEntity = null, 
			Texture2D suppliedShaderTexture = null, Texture2D suppliedSubjectTexture = null, Texture2D suppliedGlowmaskTexture = null)
		{
			if (Properties.SkipDrawing) return;

			TryGettingDrawData(rotation, scale);
			if (Entity is Item item) LoadAssets(item);
			else Loot.Logger.Warn("Could not identify shader entity identity as item");

			var useSubjectTexture = suppliedSubjectTexture ?? SubjectTexture;
			var useShaderTexture = suppliedShaderTexture ?? ShaderTexture;
			var useGlowmaskTexture = suppliedGlowmaskTexture;
			// Assets present
			if (useSubjectTexture != null && useShaderTexture != null)
			{
				// Draw the subject based on the drawlayer
				if (Properties.DrawLayer == ShaderDrawLayer.Back)
				{
					DoDrawShader(useShaderTexture, spriteBatch, lightColor);
					DoDrawSubject(useSubjectTexture, spriteBatch, lightColor);
					DoDrawGlowmask(glowmaskEntity, spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI, useGlowmaskTexture);
				}
				else if (Properties.DrawLayer == ShaderDrawLayer.Middle)
				{
					DoDrawSubject(useSubjectTexture, spriteBatch, lightColor);
					DoDrawShader(useShaderTexture, spriteBatch, lightColor);
					DoDrawGlowmask(glowmaskEntity, spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI, useGlowmaskTexture);
				}
				else if (Properties.DrawLayer == ShaderDrawLayer.Front)
				{
					DoDrawSubject(useSubjectTexture, spriteBatch, lightColor);
					DoDrawGlowmask(glowmaskEntity, spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI, useGlowmaskTexture);
					DoDrawShader(useShaderTexture, spriteBatch, lightColor);
				}
			}

			Properties.SkipUpdatingDrawData = false;
		}

		/// <summary>
		/// Will draw the glowmask
		/// </summary>
		public void DoDrawGlowmask(GlowmaskEntity glowmaskEntity, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI, Texture2D suppliedGlowmask = null)
		{
			if (glowmaskEntity == null) return;
			glowmaskEntity.Properties.SkipDrawingGlowmask = Properties.SkipDrawingGlowmask;
			glowmaskEntity.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI, suppliedGlowmask);
		}

		/// <summary>
		/// Will draw the shader effect
		/// </summary>
		public void DoDrawShader(Texture2D shaderTexture, SpriteBatch spriteBatch, Color lightColor)
		{
			if (Properties.SkipDrawingShader) return;

			var drawDataTexture = DrawData.texture;
			var drawDataColor = DrawData.color;

			DrawData.texture = shaderTexture;
			DrawData.color = lightColor;
			TryUpdatingDrawData(shaderTexture);

			spriteBatch.BeginShaderBatch();
			GameShaders.Armor.Apply(ShaderId, Entity, DrawData);

			var centerPos = DrawData.position;
			Properties.ShaderDrawStyle.DrawShader(spriteBatch, this);

			DrawData.position = centerPos;
			DrawData.texture = drawDataTexture;
			DrawData.color = drawDataColor;
			spriteBatch.ResetBatch();
		}

		/// <summary>
		/// Will draw the entity's regular sprite
		/// </summary>
		public void DoDrawSubject(Texture2D subjectTexture, SpriteBatch spriteBatch, Color lightColor)
		{
			if (Properties.SkipDrawingSubject) return;

			var drawDataColor = DrawData.color;
			var drawDataTexture = DrawData.texture;

			DrawData.color = lightColor;
			DrawData.texture = subjectTexture;
			TryUpdatingDrawData(subjectTexture);
			DrawEntity(spriteBatch);

			DrawData.color = drawDataColor;
			DrawData.texture = drawDataTexture;
		}
	}
}
