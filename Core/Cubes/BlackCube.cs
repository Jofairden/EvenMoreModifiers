using System.Net.Mail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Loot.Core.Cubes
{
	public class BlackCube : MagicalCube
	{
		protected override string CubeName => "Black Cube";
		protected override bool DisplayTier => true;

		protected override void SafeDefaults()
		{
			item.value = Item.buyPrice(gold:2);
		}

		protected override void SafeStaticDefaults()
		{
		}

		public override void RightClick(Player player)
		{
			base.RightClick(player); // base
			
			// set forced rolls to always roll 4 lines
			// set forced strength minimum 25%
			
		}
	}

	public class NPCTest : ModNPC
	{
		public override void SetDefaults()
		{
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Vector2 frameSize = new Vector2(318f, 298f);
			Texture2D texture = mod.GetTexture(this.Texture);
			
			spriteBatch.Draw
			(
				texture,
				npc.Center - Main.screenPosition,
				new Rectangle(npc.frame.X, npc.frame.Y, (int)frameSize.X, (int)frameSize.Y),
				drawColor,
				npc.rotation,
				frameSize * 0.5f,
				npc.scale, 
				SpriteEffects.None, 
				0f
			);
			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			base.PostDraw(spriteBatch, drawColor);
		}
	}
}

