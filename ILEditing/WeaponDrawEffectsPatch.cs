using System.Linq;
using Loot.Api.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Loot.ILEditing
{
	/// <summary>
	/// This patch makes custom effect draw on items being used
	/// </summary>
	internal class WeaponDrawEffectsPatch : ILEdit
	{
		public override void Apply(bool dedServ)
		{
			if (!dedServ)
				On.Terraria.DataStructures.DrawData.Draw += DrawDataOnDraw;
		}

		private void DrawDataOnDraw(On.Terraria.DataStructures.DrawData.orig_Draw orig, ref DrawData self, SpriteBatch sb)
		{
			//bool weaponOutCheck = Main.LocalPlayer.HeldItem.modItem?.mod is Mod mod && mod.Name.Equals("WeaponOut") && Main.LocalPlayer.controlUseItem || (Main.mouseLeft || Main.mouseRight);
			if (!Main.LocalPlayer.frozen
				&& (Main.LocalPlayer.itemAnimation > 0 && Main.LocalPlayer.HeldItem.useStyle != 0 || Main.LocalPlayer.HeldItem.holdStyle > 0 && !Main.LocalPlayer.pulley) // || weaponOutCheck
				&& Main.LocalPlayer.HeldItem.type > 0 && !Main.LocalPlayer.dead && !Main.LocalPlayer.HeldItem.noUseGraphic && (!Main.LocalPlayer.wet || !Main.LocalPlayer.HeldItem.noWet)
				&& Main.itemTexture[Main.LocalPlayer.HeldItem.type] == self.texture)
			{
				var info = GraphicsGlobalItem.GetInfo(Main.LocalPlayer.HeldItem);
				if (!DrawShaderEntities(info, self, sb) && !DrawGlowmaskEntities(info, self, sb))
					orig(ref self, sb);
			}
			else
			{
				orig(ref self, sb);
			}
		}

		private bool DrawShaderEntities(GraphicsGlobalItem info, DrawData self, SpriteBatch sb)
		{
			var lightColor = Lighting.GetColor((int)(Main.LocalPlayer.MountedCenter.X / 16), (int)(Main.LocalPlayer.MountedCenter.Y / 16));
			for (int i = 0; i < info.ShaderEntities.Count; i++)
			{
				var entity = info.ShaderEntities[i];
				if (entity == null) continue;
				entity.Properties.SkipUpdatingDrawData = true;
				entity.DrawData = self;
				entity.DoDrawLayeredEntity(sb, lightColor, self.color, self.scale.LengthSquared(), self.rotation, info.GlowmaskEntities[i]);
			}
			return info.HasShaders;
		}

		private bool DrawGlowmaskEntities(GraphicsGlobalItem info, DrawData self, SpriteBatch sb)
		{
			var lightColor = Lighting.GetColor((int)(Main.LocalPlayer.MountedCenter.X / 16), (int)(Main.LocalPlayer.MountedCenter.Y / 16));
			foreach (var entity in info.GlowmaskEntities.Where(x => x != null))
			{
				entity.Properties.SkipUpdatingDrawData = true;
				entity.DrawData = self;
				entity.DoDrawGlowmask(sb, lightColor, self.color, self.rotation, self.scale.LengthSquared(), entity.Entity.whoAmI);
			}

			return info.HasGlowmasks;
		}
	}
}
