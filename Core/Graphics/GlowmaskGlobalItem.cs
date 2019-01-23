using System.Linq;
using Loot.Core.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Graphics
{
	public class GlowmaskGlobalItem : GlobalItem
	{
		public GlowmaskEntity[] GlowmaskEntities;
		public bool NeedsUpdate;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public void UpdateEntities(Item item)
		{
			bool updateEntitiesArray = false;
			var pool = EMMItem.GetActivePool(item).ToArray();

			if (NeedsUpdate || GlowmaskEntities == null || GlowmaskEntities.Any(x => x != null && x.NeedsUpdate))
			{
				GlowmaskEntities = new GlowmaskEntity[pool.Length];
				updateEntitiesArray = true;
				NeedsUpdate = false;
			}

			if (updateEntitiesArray)
			{
				for (int i = 0; i < pool.Length; i++)
				{
					Modifier m = pool[i];
					GlowmaskEntities[i] = m.GetGlowmaskEntity(item);
				}

				GlowmaskEntities = GlowmaskEntities.OrderBy(x => x.Order).ToArray();
			}
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			ShaderGlobalItem shaderInfo = item.GetGlobalItem<ShaderGlobalItem>();
			GlowmaskGlobalItem glowmaskInfo = item.GetGlobalItem<GlowmaskGlobalItem>();
			glowmaskInfo.UpdateEntities(item);
			for (int i = 0; i < glowmaskInfo.GlowmaskEntities.Length; i++)
			{
				GlowmaskEntity glowmaskEntity = glowmaskInfo.GlowmaskEntities[i];
				if (glowmaskEntity != null && shaderInfo.ShaderEntities[i] == null)
				{
					glowmaskEntity.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
					glowmaskEntity.DoDrawHitbox(spriteBatch);
				}
			}
		}
	}
}
