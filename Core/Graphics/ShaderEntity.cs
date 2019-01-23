using Loot.Core.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace Loot.Core.Graphics
{
	public class ShaderEntity : GraphicsEntity
	{
		public DrawData DrawData;
		public bool DrawShader { get; set; }
		public ShaderDrawLayer DrawLayer { get; set; }
		public ShaderDrawOffsetStyle DrawOffsetStyle { get; set; }
		public int ShaderId { get; set; }
		public int NumSegments { get; set; }
		public int DrawDistance { get; set; }
		public Color DrawColor { get; set; }
		public Texture2D SubjectTexture { get; protected set; }
		public Texture2D ShaderTexture { get; protected set; }
		public bool NeedsUpdate { get; set; }
		public bool UseDestinationRectangle { get; set; }
		public Rectangle? DestinationRectangle { get; set; }
		public short Order { get; set; }

		public ShaderEntity(object subjectIdentity, int shaderId, bool drawShader = true, ShaderDrawLayer drawLayer = ShaderDrawLayer.Back, ShaderDrawOffsetStyle drawOffsetStyle = ShaderDrawOffsetStyle.Default,
			int numSegments = 8, int drawDistance = 4, Color? shaderDrawColor = null, bool useDestinationRectangle = true, short order = 0)
			: base(subjectIdentity)
		{
			ShaderId = shaderId;
			DrawShader = drawShader;
			DrawLayer = drawLayer;
			DrawOffsetStyle = drawOffsetStyle;
			NumSegments = numSegments;
			DrawDistance = drawDistance;
			DrawColor = shaderDrawColor ?? Color.White;
			NeedsUpdate = false;
			UseDestinationRectangle = useDestinationRectangle;
			DestinationRectangle = null;
			Order = 0;
		}

		/// <summary>
		/// Gets the draw offset for the shader
		/// Different styles can be programmed to achieve different looks
		/// </summary>
		/// <param name="i">The i-th segment</param>
		public virtual Vector2 GetDrawOffset(int i)
		{
			if (DrawOffsetStyle == ShaderDrawOffsetStyle.Default)
			{
				return new Vector2(0, DrawDistance).RotatedBy((float) i / NumSegments * MathHelper.TwoPi);
			}

			if (DrawOffsetStyle == ShaderDrawOffsetStyle.Alternate)
			{
				var halfDist = DrawDistance / 2;
				var offY = halfDist + halfDist * i % halfDist;
				return new Vector2(0, offY).RotatedBy((float) i / NumSegments * MathHelper.TwoPi);
			}

			return Vector2.Zero;
		}

		/// <summary>
		/// Sets up drawing data initially
		/// </summary>
		public void TryGettingDrawData(float rotation, float scale)
		{
			DrawData = new DrawData
			{
				color = DrawColor,
				effect = SpriteEffects.None,
				rotation = rotation,
				scale = new Vector2(scale, scale)
			};

			//if (Main.LocalPlayer.gravDir == -1)
			//	DrawData.effect |= SpriteEffects.FlipVertically;
			if (Entity.direction == -1)
			{
				DrawData.effect |= SpriteEffects.FlipHorizontally;
			}
		}

		// TODO custom animation logic
		public void TryUpdatingDrawData(Texture2D texture)
		{
			var frame = texture.Frame();
			DrawData.position = new Vector2
			(
				Entity.position.X - Main.screenPosition.X + Entity.width * 0.5f,
				Entity.position.Y - Main.screenPosition.Y + Entity.height - texture.Height * 0.5f + 2f
			);
			DrawData.texture = texture;
			DrawData.origin = frame.Size() * 0.5f;

			if (UseDestinationRectangle)
			{
				if (DestinationRectangle.HasValue)
				{
					DrawData.destinationRectangle = DestinationRectangle.Value;
				}
				else
				{
					DrawData.destinationRectangle = frame;

					if (Entity is NPC)
					{
						DrawData.destinationRectangle = ((NPC) Entity).frame;
					}
					else if (Entity is Projectile)
					{
						DrawData.destinationRectangle.Y = ((Projectile) Entity).frame * Entity.height;
					}
				}
			}
		}

		// @todo dynamic load assets
		protected void LoadAssets(Item item)
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
			if (DrawShader)
			{
				TryGettingDrawData(rotation, scale);
				LoadAssets((Item) Entity);

				// Assets present
				if (SubjectTexture != null && ShaderTexture != null)
				{
					// Draw the subject based on the drawlayer
					if (DrawLayer == ShaderDrawLayer.Back)
					{
						DoDrawShader(ShaderTexture, spriteBatch);
						DoDrawSubject(SubjectTexture, spriteBatch, lightColor, alphaColor);
						glowmaskEntity?.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI);
					}
					else if (DrawLayer == ShaderDrawLayer.Middle)
					{
						DoDrawSubject(SubjectTexture, spriteBatch, lightColor, alphaColor);
						DoDrawShader(ShaderTexture, spriteBatch);
						glowmaskEntity?.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI);
					}
					else if (DrawLayer == ShaderDrawLayer.Front)
					{
						DoDrawSubject(SubjectTexture, spriteBatch, lightColor, alphaColor);
						glowmaskEntity?.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, Entity.whoAmI);
						DoDrawShader(ShaderTexture, spriteBatch);
					}
				}
			}
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
				DrawData.position = centerPos + GetDrawOffset(i);
				DrawData.Draw(spriteBatch);
			}

			DrawData.position = centerPos;
			spriteBatch.ResetBatch();
		}

		/// <summary>
		/// Will draw the entity's regular sprite
		/// </summary>
		public void DoDrawSubject(Texture2D subjectTexture, SpriteBatch spriteBatch, Color lightColor, Color alphaColor)
		{
			TryUpdatingDrawData(subjectTexture);
			spriteBatch.Draw(subjectTexture, DrawData.position, DrawData.destinationRectangle, lightColor, DrawData.rotation, DrawData.origin, DrawData.scale, DrawData.effect, 0f);
		}
	}
}
