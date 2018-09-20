using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Graphics
{
	public class ShaderGlobalItem : GlobalItem
	{
		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			bool flag = true;
			foreach (var modifier in EMMItem.GetActivePool(item))
			{
				var entity = modifier.GetShaderEntity(item);
				if (entity != null)
				{
					flag = false;
					entity.DoDrawLayeredEntity(spriteBatch, lightColor, alphaColor, scale, rotation, modifier.GetGlowmaskEntity(item));
				}
			}

			return flag;
		}
	}
}
