using Loot.Api.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Loot.Api.Graphics.Glowmask
{
	/// <summary>
	/// Defines a Glowmask entity, part of <see cref="GraphicsEntity"/>
	/// The entity defines a particular 'glowmask' "of" the entity
	/// and can be drawn appropriately
	/// The entity makes use of <see cref="GraphicsModContent"/> to get
	/// art assets
	/// </summary>
	public class GlowmaskEntity : GraphicsEntity<GlowmaskGraphicsProperties>
	{
		public bool DrawHitbox { get; set; } // mostly for debug purposes
		public Texture2D GlowmaskTexture { get; protected set; }

		public GlowmaskEntity(object subjectIdentity, bool drawHitbox = false, short order = 0, Color? drawColor = null,
			GlowmaskGraphicsProperties props = null)
			: base(subjectIdentity)
		{
			DrawHitbox = drawHitbox;
			Order = order;
			DrawColor = drawColor ?? Color.White;
			Properties = props ?? GlowmaskGraphicsProperties.Builder.Build();
		}

		protected override void LoadAssets(Item item)
		{
			if (GlowmaskTexture != null) return;
			var graphicsContent = Loot.ModContentManager.GetContent<GraphicsModContent>();
			graphicsContent?.Prepare(item);
			GlowmaskTexture = graphicsContent?.GetPreparedGlowmask(item.type.ToString()) ?? Main.itemTexture[item.type];
		}

		/// <summary>
		/// Will draw the glowmask texture
		/// </summary>
		public void DoDrawGlowmask(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI, Texture2D suppliedGlowmask = null)
		{
			if (!Properties.SkipDrawing || !Properties.SkipDrawingGlowmask) return;

			TryGettingDrawData(rotation, scale);
			if (Entity is Item item) LoadAssets(item);
			else Loot.Logger.Warn("Could not identify shader entity identity as item");
			Texture2D useGlowmaskTexture = suppliedGlowmask ?? GlowmaskTexture;

			if (useGlowmaskTexture != null)
			{
				var drawDataTexture = DrawData.texture;
				var drawDataColor = DrawData.color;
				var drawDataDestinationRectangle = DrawData.destinationRectangle;
				DrawData.texture = useGlowmaskTexture;
				DrawData.color = lightColor;
				TryUpdatingDrawData(useGlowmaskTexture);
				DrawEntity(spriteBatch);
				DrawData.texture = drawDataTexture;
				DrawData.color = drawDataColor;
				DrawData.destinationRectangle = drawDataDestinationRectangle;
			}

			Properties.SkipUpdatingDrawData = false;
		}

		/// <summary>
		/// Will draw the hitbox around the entity
		/// </summary>
		public void DoDrawHitbox(SpriteBatch spriteBatch)
		{
			if (!DrawHitbox) return;
			Rectangle hitbox = Entity.Hitbox;
			hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
			spriteBatch.Draw(Main.magicPixel, hitbox, Color.White);
		}
	}
}
