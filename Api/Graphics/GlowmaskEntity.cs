using Loot.Api.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Loot.Api.Graphics
{
	/// <summary>
	/// Defines a Glowmask entity, part of <see cref="GraphicsEntity"/>
	/// The entity defines a particular 'glowmask' "of" the entity
	/// and can be drawn appropriately
	/// The entity makes use of <see cref="GraphicsModContent"/> to get
	/// art assets
	/// </summary>
	public class GlowmaskEntity : GraphicsEntity
	{
		public bool DrawHitbox { get; set; } // mostly for debug purposes
		public bool NeedsUpdate { get; set; }
		public Texture2D GlowmaskTexture { get; protected set; }
		public short Order { get; set; }

		public GlowmaskEntity(object subjectIdentity, bool drawHitbox = false, short order = 0, Color? drawColor = null)
			: base(subjectIdentity)
		{
			DrawHitbox = drawHitbox;
			NeedsUpdate = false;
			Order = order;
			DrawColor = drawColor ?? Color.White;
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
			if (!SkipDrawing) return;

			Texture2D drawTexture = suppliedGlowmask;
			if (drawTexture == null)
			{
				LoadAssets((Item)Entity);
				drawTexture = GlowmaskTexture;
			}

			if (drawTexture != null)
			{
				TryGettingDrawData(rotation, scale);	
				var drawDataTexture = DrawData.texture;
				var drawDataColor = DrawData.color;
				var drawDataDestinationRectangle = DrawData.destinationRectangle;
				TryUpdatingDrawData(drawTexture);
				DrawDrawData(spriteBatch, DrawData);
				DrawData.texture = drawDataTexture;
				DrawData.color = drawDataColor;
				DrawData.destinationRectangle = drawDataDestinationRectangle;
			}

			SkipUpdatingDrawData = false;
		}

		/// <summary>
		/// Will draw the hitbox around the entity
		/// </summary>
		public void DoDrawHitbox(SpriteBatch spriteBatch)
		{
			if (DrawHitbox)
			{
				Rectangle hitbox = Entity.Hitbox;
				hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
				spriteBatch.Draw(Main.magicPixel, hitbox, Color.White);
			}
		}
	}
}
