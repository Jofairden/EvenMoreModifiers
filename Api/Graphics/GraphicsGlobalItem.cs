using System.Collections.Generic;
using System.Linq;
using Loot.Api.Graphics.Glowmask;
using Loot.Api.Graphics.Shader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Api.Graphics
{
	/// <summary>
	/// This class is responsible for handling <see cref="ShaderEntity"/> and <see cref="GlowmaskEntity"/>
	/// that come from the <see cref="Modifier"/>s on a given item
	/// </summary>
	public class GraphicsGlobalItem : GlobalItem
	{
		public List<ShaderEntity> ShaderEntities { get; private set; } = new List<ShaderEntity>();
		public List<GlowmaskEntity> GlowmaskEntities { get; private set; } = new List<GlowmaskEntity>();
		public bool NeedsUpdate = true;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public static GraphicsGlobalItem GetInfo(Item item) => item.GetGlobalItem<GraphicsGlobalItem>();

		public bool HasShaders => ShaderEntities.Count(x => x != null) > 0;
		public bool HasGlowmasks => GlowmaskEntities.Count(x => x != null) > 0;

		public override void UpdateInventory(Item item, Player player)
		{
			UpdateEntities(item);
		}

		public override void UpdateEquip(Item item, Player player)
		{
			UpdateEntities(item);
		}

		public void UpdateEntities(Item item)
		{
			var pool = LootModItem.GetActivePool(item).ToArray();

			if (!NeedsUpdate && ShaderEntities != null
							 && !ShaderEntities.Any(x => x != null && x.NeedsUpdate)
							 && !GlowmaskEntities.Any(x => x != null && x.NeedsUpdate))
				return;

			if (ShaderEntities != null)
			{
				ShaderEntities.Clear();

				foreach (var m in pool)
				{
					var ent = m.GetShaderEntity(item);
					ent?.SetIdentity(item);
					ShaderEntities.Add(ent);
				}

				ShaderEntities = new List<ShaderEntity>(ShaderEntities.OrderBy(x => x?.Order ?? 0));
			}

			if (GlowmaskEntities != null)
			{
				GlowmaskEntities.Clear();

				foreach (var m in pool)
				{
					var ent = m.GetGlowmaskEntity(item);
					ent?.SetIdentity(item);
					GlowmaskEntities.Add(ent);
				}

				GlowmaskEntities = new List<GlowmaskEntity>(GlowmaskEntities.OrderBy(x => x?.Order ?? 0));
			}

			NeedsUpdate = false;
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			bool flag = true;
			UpdateEntities(item);

			for (int i = 0; i < ShaderEntities.Count; i++)
			{
				ShaderEntity entity = ShaderEntities[i];
				if (entity != null)
				{
					flag = false;
					entity.DoDrawLayeredEntity(spriteBatch, lightColor, alphaColor, scale, rotation, GlowmaskEntities[i]);
				}
			}

			return flag;
		}

		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (HasShaders) return;

			foreach (var entity in GlowmaskEntities)
			{
				if (entity != null)
				{
					entity.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
					entity.DoDrawHitbox(spriteBatch);
				}
			}
		}
	}
}
