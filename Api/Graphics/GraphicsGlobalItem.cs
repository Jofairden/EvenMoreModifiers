using System.Collections.Generic;
using System.Linq;
using Loot.Api.Core;
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
		public List<ShaderEntity> ShaderEntities { get; internal set; } = new List<ShaderEntity>();
		public List<GlowmaskEntity> GlowmaskEntities { get; internal set; } = new List<GlowmaskEntity>();

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public static GraphicsGlobalItem GetInfo(Item item) => item.GetGlobalItem<GraphicsGlobalItem>();

		public bool HasShaders => ShaderEntities.Count(x => x != null) > 0;
		public bool HasGlowmasks => GlowmaskEntities.Count(x => x != null) > 0;

		public static void UpdateGraphicsEntities(List<Modifier> modifiers, Item item)
		{
			var info = item.GetGlobalItem<GraphicsGlobalItem>();
			info.ShaderEntities = modifiers.Select(mod => mod.GetShaderEntity(item)).OrderBy(mod => mod?.Order ?? 0).ToList();
			info.GlowmaskEntities = modifiers.Select(mod => mod.GetGlowmaskEntity(item)).OrderBy(mod => mod?.Order ?? 0).ToList();
			var clone = item.Clone();
			info.ShaderEntities.ForEach(mod => mod?.SetIdentity(clone));
			info.GlowmaskEntities.ForEach(mod => mod?.SetIdentity(clone));
		}

		public override void UpdateEquip(Item item, Player player)
		{
			UpdateIdentity(item);
		}

		public override void UpdateInventory(Item item, Player player)
		{
			UpdateIdentity(item);
		}

		public void UpdateIdentity(Item item)
		{
			ShaderEntities.ForEach(entity => entity?.SetIdentity(item));
			GlowmaskEntities.ForEach(entity => entity?.SetIdentity(item));
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			bool flag = true;
			UpdateIdentity(item);

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
