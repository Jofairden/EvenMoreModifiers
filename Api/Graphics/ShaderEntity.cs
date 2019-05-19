using Loot.Api.Ext;
using Loot.Api.Graphics.Shaders;
using Loot.Api.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Loot.Api.Graphics
{
	/// <summary>
	/// Defines a Shader entity, part of <see cref="GraphicsEntity"/>
	/// The entity defines a particular 'shader' "of" the entity
	/// and can be drawn appropriately
	/// The entity makes use of <see cref="GraphicsModContent"/> to get
	/// art assets
	/// </summary>
	public class ShaderEntity : GraphicsEntity
	{
		public ShaderDrawLayer DrawLayer { get; set; }
		public ShaderDrawStyle ShaderDrawStyle { get; set; }
		public int ShaderId { get; set; }
		public int NumSegments { get; set; }
		public int DrawDistance { get; set; }
		public Texture2D SubjectTexture { get; protected set; }
		public Texture2D ShaderTexture { get; protected set; }
		public bool NeedsUpdate { get; set; }
		public short Order { get; set; }

		public ShaderEntity(object subjectIdentity, int shaderId, ShaderDrawLayer drawLayer = ShaderDrawLayer.Back, ShaderDrawStyle shaderDrawStyle = null,
			int numSegments = 8, int drawDistance = 4, Color? shaderDrawColor = null, short order = 0)
			: base(subjectIdentity)
		{
			ShaderId = shaderId;
			DrawLayer = drawLayer;
			ShaderDrawStyle = shaderDrawStyle ?? ShaderDrawStyle.Styles.Default;
			NumSegments = numSegments;
			DrawDistance = drawDistance;
			DrawColor = shaderDrawColor ?? Color.White;
			NeedsUpdate = false;
			Order = 0;
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
				ShaderTexture = graphicsContent?.GetPreparedShader(item.type.ToString()) ?? Main.itemTexture[item.type];
			}
		}

		/// <summary>
		/// Will draw all the layers (subject, glowmask, shader) in the proper order 
		/// </summary>
		public void DoDrawLayeredEntity(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float scale, float rotation, GlowmaskEntity glowmaskEntity = null)
		{
			if (SkipDrawing) return;

			TryGettingDrawData(rotation, scale);
			LoadAssets((Item)Entity);

			// Assets present
			if (SubjectTexture != null && ShaderTexture != null)
			{
				// Draw the subject based on the drawlayer
				if (DrawLayer == ShaderDrawLayer.Back)
				{
					DoDrawShader(ShaderTexture, spriteBatch);
					DoDrawSubject(SubjectTexture, spriteBatch, lightColor);
					glowmaskEntity?.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI);
				}
				else if (DrawLayer == ShaderDrawLayer.Middle)
				{
					DoDrawSubject(SubjectTexture, spriteBatch, lightColor);
					DoDrawShader(ShaderTexture, spriteBatch);
					glowmaskEntity?.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI);
				}
				else if (DrawLayer == ShaderDrawLayer.Front)
				{
					DoDrawSubject(SubjectTexture, spriteBatch, lightColor);
					glowmaskEntity?.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI);
					DoDrawShader(ShaderTexture, spriteBatch);
				}
			}

			SkipUpdatingDrawData = false;
		}

		/// <summary>
		/// Will draw the shader effect
		/// </summary>
		public void DoDrawShader(Texture2D shaderTexture, SpriteBatch spriteBatch)
		{
			TryUpdatingDrawData(shaderTexture);
			spriteBatch.BeginShaderBatch();
			GameShaders.Armor.Apply(ShaderId, Entity, DrawData);
			var centerPos = DrawData.position;

			for (int i = 0; i < NumSegments; i++)
			{
				DrawData.position = centerPos + ShaderDrawStyle.GetDrawOffset(this, i);
				DrawDrawData(spriteBatch, DrawData);
			}

			DrawData.position = centerPos;
			spriteBatch.ResetBatch();
		}

		/// <summary>
		/// Will draw the entity's regular sprite
		/// </summary>
		public void DoDrawSubject(Texture2D subjectTexture, SpriteBatch spriteBatch, Color lightColor)
		{
			var drawDataColor = DrawData.color;
			var drawDataTexture = DrawData.texture;
			TryUpdatingDrawData(subjectTexture);
			DrawData.color = lightColor;
			DrawData.texture = subjectTexture;
			DrawDrawData(spriteBatch, DrawData);
			DrawData.color = drawDataColor;
			DrawData.texture = drawDataTexture;
		}
	}
}
