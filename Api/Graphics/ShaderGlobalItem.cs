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
	/// This class is responsible for handling <see cref="ShaderEntity"/>
	/// that come from the <see cref="Modifier"/>s on a given item
	/// </summary>
	public class ShaderGlobalItem : GlobalItem
	{
		public List<ShaderEntity> ShaderEntities = new List<ShaderEntity>();
		public bool NeedsUpdate;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public void UpdateEntities(Item item)
		{
			var pool = LootModItem.GetActivePool(item).ToArray();

			if (!NeedsUpdate && ShaderEntities != null && !ShaderEntities.Any(x => x != null && x.NeedsUpdate))
			{
				return;
			}

			if (ShaderEntities != null)
			{
				ShaderEntities.Clear();

				foreach (var m in pool)
				{
					var ent = m.GetShaderEntity(item);
					ShaderEntities.Add(ent);
				}

				ShaderEntities = new List<ShaderEntity>(ShaderEntities.OrderBy(x => x?.Order ?? 0));
			}

			NeedsUpdate = false;
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			bool flag = true;
			ShaderGlobalItem shaderInfo = item.GetGlobalItem<ShaderGlobalItem>();
			GlowmaskGlobalItem glowmaskInfo = item.GetGlobalItem<GlowmaskGlobalItem>();
			shaderInfo.UpdateEntities(item);
			glowmaskInfo.UpdateEntities(item);

			for (int i = 0; i < shaderInfo.ShaderEntities.Count; i++)
			{
				ShaderEntity shaderEntity = shaderInfo.ShaderEntities[i];
				if (shaderEntity != null)
				{
					flag = false;
					shaderEntity.DoDrawLayeredEntity(spriteBatch, lightColor, alphaColor, scale, rotation, glowmaskInfo.GlowmaskEntities[i]);
				}
			}

			return flag;
		}
	}
}
