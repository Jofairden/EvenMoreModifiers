using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Loot.Api.Graphics
{
	/// <summary>
	/// Defines the base abstract of a GraphicsEntity
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class GraphicsEntity<T> where T : GraphicsProperties
	{
		public DrawData DrawData;
		public Color DrawColor { get; set; }
		public bool UseDestinationRectangle => DrawData.useDestinationRectangle;
		public Rectangle? DestinationRectangle => DrawData.destinationRectangle;

		public Entity Entity { get; protected set; }
		public T Properties { get; protected set; }
		public short Order { get; protected set; }
		public bool NeedsUpdate { get; set; } = true;

		protected GraphicsEntity(object subjectIdentity)
		{
			if (subjectIdentity == Entity) return;
			SetIdentity(subjectIdentity);
		}

		internal void SetIdentity(object subjectIdentity)
		{
			if (subjectIdentity is ModItem modItem)
			{
				Entity = modItem.item;
			}
			else if (subjectIdentity is ModNPC modNpc)
			{
				Entity = modNpc.npc;
			}
			else if (subjectIdentity is ModProjectile modProjectile)
			{
				Entity = modProjectile.projectile;
			}
			else if (subjectIdentity is ModPlayer modPlayer)
			{
				Entity = modPlayer.player;
			}
			else if (subjectIdentity is Entity entity)
			{
				Entity = entity;
			}
			else
			{
				throw new Exception($"Invalid subject for GraphicsEntityIdentity - {subjectIdentity}");
			}
		}

		protected abstract void LoadAssets(Item item);

		/// <summary>
		/// Sets up drawing data initially
		/// </summary>
		public void TryGettingDrawData(float rotation, float scale)
		{
			if (Properties.SkipUpdatingDrawData && !NeedsUpdate) return;

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

		private float offY;
		private float offX;

		public void TryUpdatingDrawData(Texture2D texture)
		{
			if (Properties.SkipUpdatingDrawData && !NeedsUpdate) return;

			var frame = texture.Frame();
			offX = Main.screenPosition.X - texture.Width * 0.5f;
			offY = Main.screenPosition.Y - texture.Height * 0.5f;
			DrawData.position = new Vector2
			(
				Entity.position.X - offX,
				Entity.position.Y - offY
			);
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

					if (Entity is NPC npc)
					{
						DrawData.destinationRectangle = npc.frame;
					}
					else if (Entity is Projectile projectile)
					{
						DrawData.destinationRectangle.Y = projectile.frame * projectile.height;
					}
				}
			}
		}

		public void DrawEntity(SpriteBatch spriteBatch)
		{
			if (DrawData.useDestinationRectangle)
			{
				spriteBatch.Draw(DrawData.texture, DrawData.destinationRectangle, DrawData.sourceRect, DrawData.color, DrawData.rotation, DrawData.origin, DrawData.effect, 0f);
			}
			else
			{
				spriteBatch.Draw(DrawData.texture, DrawData.position, DrawData.sourceRect, DrawData.color, DrawData.rotation, DrawData.origin, DrawData.scale, DrawData.effect, 0f);
			}
			NeedsUpdate = false;
		}
	}
}
