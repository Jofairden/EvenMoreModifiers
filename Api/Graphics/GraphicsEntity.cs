using System;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Api.Graphics
{
	public abstract class GraphicsEntity
	{
		public object Identity { get; protected set; }
		public Entity Entity { get; protected set; }

		protected GraphicsEntity(object subjectIdentity)
		{
			if (subjectIdentity == Identity) return;
			SetIdentity(subjectIdentity);
		}

		internal void SetIdentity(object subjectIdentity)
		{
			Identity = subjectIdentity;
			if (subjectIdentity is Entity entity)
			{
				Entity = entity;
			}
			else if (subjectIdentity is ModItem modItem)
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
			else
			{
				throw new Exception($"Invalid subject for GraphicsEntityIdentity - {subjectIdentity}");
			}
		}
	}
}
