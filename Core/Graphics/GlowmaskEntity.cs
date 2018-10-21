using Loot.Core.ModContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Loot.Core.Graphics
{
	public class GlowmaskEntity : GraphicsEntity
	{
		public bool DrawHitbox { get; set; } // mostly for debug purposes
		public bool DrawGlowmask { get; set; }
		public bool NeedsUpdate { get; set; }
		public Texture2D GlowmaskTexture { get; protected set; }
		public bool UseDestinationRectangle { get; set; }
		public Rectangle? DestinationRectangle { get; set; }

		public GlowmaskEntity(object subjectIdentity, bool drawHitbox = false, bool drawGlowmask = true, bool useDestinationRectangle = true) 
			: base(subjectIdentity)
		{
			DrawHitbox = drawHitbox;
			DrawGlowmask = drawGlowmask;
			NeedsUpdate = false;
			UseDestinationRectangle = useDestinationRectangle;
			DestinationRectangle = null;
		}

		// TODO custom animation logic
		public Rectangle? GetFrameRectangle(Texture2D texture)
		{
			if (UseDestinationRectangle)
			{
				if (DestinationRectangle.HasValue)
					return DestinationRectangle.Value;

				Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
				if (Entity is NPC)
				{
					rectangle = ((NPC)Entity).frame;
				}
				else if (Entity is Projectile)
				{
					rectangle.Y = ((Projectile)Entity).frame * Entity.height;
				}

				return rectangle;
			}
			return null;
		}

		protected void LoadAssets(Item item)
		{
			if (GlowmaskTexture != null) return;
			var graphicsContent = Loot.ModContentManager.GetContent<GraphicsModContent>();
			graphicsContent?.Prepare(item);
			GlowmaskTexture = graphicsContent?.GetPreparedGlowmask(item.type.ToString());
			if (GlowmaskTexture == null)
			{
				GlowmaskTexture = Main.itemTexture[item.type];
			}
		}

		/// <summary>
		/// Will draw the glowmask texture
		/// </summary>
		public void DoDrawGlowmask(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI, Texture2D suppliedGlowmask = null)
		{
			if (DrawGlowmask)
			{
				Texture2D drawTexture = suppliedGlowmask;
				if (drawTexture == null)
				{
					LoadAssets((Item)Entity);
					drawTexture = GlowmaskTexture;
				}

				// Asset is present
				if (drawTexture != null)
				{
					SpriteEffects effect = SpriteEffects.None;
					//if (Main.LocalPlayer.gravDir == -1)
					//	effect |= SpriteEffects.FlipVertically;
					if (Entity.direction == -1)
						effect |= SpriteEffects.FlipHorizontally;
					
					// Draw the glowmask
					spriteBatch.Draw
					(
						drawTexture,
						new Vector2
						(
							Entity.position.X - Main.screenPosition.X + Entity.width * 0.5f,
							Entity.position.Y - Main.screenPosition.Y + Entity.height - drawTexture.Height * 0.5f + 2f
						),
						GetFrameRectangle(drawTexture),
						Color.White,
						rotation,
						drawTexture.Size() * 0.5f,
						scale,
						effect,
						0f
					);
				}
			}
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
