using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Graphics
{
	public class ShaderGlobalItem : GlobalItem
	{
		public ShaderEntity[] ShaderEntities;
		public bool NeedsUpdate;

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public void UpdateEntities(Item item)
		{
			bool flag = false;
			var pool = EMMItem.GetActivePool(item).ToArray();

			if (NeedsUpdate || ShaderEntities == null || ShaderEntities.Any(x => x != null && x.NeedsUpdate))
			{
				ShaderEntities = new ShaderEntity[pool.Length];
				flag = true;
				NeedsUpdate = false;
			}

			if (flag)
			{
				for (int i = 0; i < pool.Length; i++)
				{
					Modifier m = pool[i];
					ShaderEntities[i] = m.GetShaderEntity(item);
				}
			}
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			bool flag = true;
			ShaderGlobalItem shaderInfo = item.GetGlobalItem<ShaderGlobalItem>();
			GlowmaskGlobalItem glowmaskInfo = item.GetGlobalItem<GlowmaskGlobalItem>();
			shaderInfo.UpdateEntities(item);
			glowmaskInfo.UpdateEntities(item);

			for (int i = 0; i < shaderInfo.ShaderEntities.Length; i++)
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
