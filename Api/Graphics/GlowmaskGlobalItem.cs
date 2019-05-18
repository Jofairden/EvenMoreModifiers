using System.Collections.Generic;
using System.Linq;
using Loot.Api.Modifier;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Api.Graphics
{
	/// <summary>
	/// This class is responsible for handling <see cref="GlowmaskEntity"/>
	/// that come from the <see cref="Modifier"/>s on a given item
	/// </summary>
	public class GlowmaskGlobalItem : GlobalItem
	{
		public List<GlowmaskEntity> GlowmaskEntities = new List<GlowmaskEntity>();
		public bool NeedsUpdate;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public void UpdateEntities(Item item)
		{
			var pool = LootModItem.GetActivePool(item).ToArray();

			if (!NeedsUpdate && GlowmaskEntities != null && !GlowmaskEntities.Any(x => x != null && x.NeedsUpdate))
			{
				return;
			}

			if (GlowmaskEntities != null)
			{
				GlowmaskEntities.Clear();

				foreach (var m in pool)
				{
					var ent = m.GetGlowmaskEntity(item);
					GlowmaskEntities.Add(ent);
				}

				GlowmaskEntities = new List<GlowmaskEntity>(GlowmaskEntities.OrderBy(x => x?.Order ?? 0));
			}

			NeedsUpdate = false;
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			ShaderGlobalItem shaderInfo = item.GetGlobalItem<ShaderGlobalItem>();
			GlowmaskGlobalItem glowmaskInfo = item.GetGlobalItem<GlowmaskGlobalItem>();
			glowmaskInfo.UpdateEntities(item);

			for (int i = 0; i < glowmaskInfo.GlowmaskEntities.Count; i++)
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
