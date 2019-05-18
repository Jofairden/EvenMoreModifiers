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
			Identity = subjectIdentity;
			if (subjectIdentity is Entity)
			{
				Entity = (Entity) subjectIdentity;
			}
			else if (subjectIdentity is ModItem)
			{
				Entity = ((ModItem) subjectIdentity).item;
			}
			else if (subjectIdentity is ModNPC)
			{
				Entity = ((ModNPC) subjectIdentity).npc;
			}
			else if (subjectIdentity is ModProjectile)
			{
				Entity = ((ModProjectile) subjectIdentity).projectile;
			}
			else if (subjectIdentity is ModPlayer)
			{
				Entity = ((ModPlayer) subjectIdentity).player;
			}
			else
			{
				throw new Exception($"Invalid subject for GraphicsEntityIdentity - {subjectIdentity}");
			}
		}
	}
}
