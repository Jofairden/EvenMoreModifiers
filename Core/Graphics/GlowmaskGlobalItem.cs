using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Graphics
{
	public class GlowmaskGlobalItem : GlobalItem
	{
		public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			foreach (var modifier in EMMItem.GetActivePool(item))
			{
				var shader = modifier.GetShaderEntity(item);
				if (shader == null)
				{
					modifier.GetGlowmaskEntity(item)?.DoDrawGlowmask(spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
					modifier.GetGlowmaskEntity(item)?.DoDrawHitbox(spriteBatch);
				}
			}
		}
	}
}
