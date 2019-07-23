using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Api.Graphics;
using Loot.Api.Graphics.Glowmask;
using Loot.Api.Graphics.Shader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Loot.ModSupport
{
	internal class WeaponOutSupport : ModSupport
	{
		public override string ModName => "WeaponOut";

		public override bool CheckValidity(Mod mod) => mod.Version >= new Version(1, 6, 4);

		public override void AddClientSupport(Mod mod)
		{
			On.Terraria.Main.DrawPlayer += MainOnDrawPlayer;
			On.Terraria.DataStructures.DrawData.Draw += DrawDataOnDraw;
			mod.Call("AddCustomPreDrawMethod", (Func<Player, Item, DrawData, bool>)CustomPreDraw);
		}

		/// <summary>
		/// Hooks into <see cref="Player.DrawPlayer"/> and will clear our cached data before calling the original
		/// <para>This ensures cached data resets for each player</para>
		/// <para>It works because Terraria draws one player at a time, so cached data is from one player</para>
		/// </summary>
		private void MainOnDrawPlayer(On.Terraria.Main.orig_DrawPlayer orig, Main self, Player drawplayer, Vector2 position, float rotation, Vector2 rotationorigin, float shadow)
		{
			_lightColor = null;
			_cache.Clear();
			orig(self, drawplayer, position, rotation, rotationorigin, shadow);
		}

		/// <summary>
		/// Intercepts <see cref="DrawData.Draw"/> calls
		/// <para>If held weapon is a fist and the DrawData matches, call <see cref="DrawWeaponOutFists"/></para>
		/// <para>If the DrawData was cached, it came from WeaponOut and call <see cref="DrawWeaponOutFromCache"/></para>
		/// </summary>
		private void DrawDataOnDraw(On.Terraria.DataStructures.DrawData.orig_Draw orig, ref DrawData self, SpriteBatch sb)
		{
			_lightColor = _lightColor ?? Lighting.GetColor((int)(Main.LocalPlayer.MountedCenter.X / 16), (int)(Main.LocalPlayer.MountedCenter.Y / 16));
			if (!DrawWeaponOutFists(self, sb) && !DrawWeaponOutFromCache(self, sb))
				orig(ref self, sb);
		}

		// Specific useStyle used for WO's fists
		private const int WEAPONOUT_FISTS_USESTYLE = 102115116;

		private bool DrawWeaponOutFists(DrawData self, SpriteBatch sb)
		{
			if (Main.LocalPlayer.HeldItem.useStyle != WEAPONOUT_FISTS_USESTYLE) return false;
			bool isHandOn = Main.LocalPlayer.handon > 0 && self.texture == Main.accHandsOnTexture[Main.LocalPlayer.handon];
			bool isHandOff = Main.LocalPlayer.handoff > 0 && self.texture == Main.accHandsOffTexture[Main.LocalPlayer.handoff];
			if (!isHandOn && !isHandOff) return false;

			var info = GraphicsGlobalItem.GetInfo(Main.LocalPlayer.HeldItem);
			if (info.HasShaders)
			{
				for (int i = 0; i < info.ShaderEntities.Count; i++)
				{
					var entity = info.ShaderEntities[i];
					if (entity == null) continue;
					entity.Properties.SkipUpdatingDrawData = true;
					entity.DrawData = self;
					entity.DoDrawLayeredEntity(sb, _lightColor.Value, self.color, self.scale.LengthSquared(), self.rotation,
						info.GlowmaskEntities[i], self.texture, self.texture, self.texture);
				}

				return true;
			}

			foreach (var entity in info.GlowmaskEntities.Where(x => x != null))
			{
				entity.Properties.SkipUpdatingDrawData = true;
				entity.DrawData = self;
				entity.DoDrawGlowmask(sb, _lightColor.Value, self.color, self.rotation, self.scale.LengthSquared(), entity.Entity.whoAmI, self.texture);
			}

			return info.HasGlowmasks;
		}

		private bool DrawWeaponOutFromCache(DrawData self, SpriteBatch sb)
		{
			var data = self;
			var cachedDatas = _cache.Where(x => x.drawData.texture == data.texture).ToList();
			if (cachedDatas.Any())
			{
				foreach (var cachedData in cachedDatas)
				{
					var shaderEntity = cachedData.shaderEntity;
					var glowmaskEntity = cachedData.glowmaskEntity;
					var plr = cachedData.player;
					_lightColor = _lightColor ?? Lighting.GetColor((int)(plr.MountedCenter.X / 16), (int)(plr.MountedCenter.Y / 16));

					if (shaderEntity != null)
					{
						shaderEntity.Properties.SkipUpdatingDrawData = true;
						shaderEntity.DrawData = self;
						shaderEntity.DoDrawLayeredEntity(sb, _lightColor.Value, self.color, self.scale.LengthSquared(), self.rotation, glowmaskEntity);
					}
					else if (glowmaskEntity != null)
					{
						glowmaskEntity.Properties.SkipUpdatingDrawData = true;
						glowmaskEntity.DrawData = self;
						glowmaskEntity.DoDrawGlowmask(sb, _lightColor.Value, self.color, self.rotation, self.scale.LengthSquared(), glowmaskEntity.Entity.whoAmI);
					}
				}

				return true;
			}

			return false;
		}

		private Color? _lightColor;
		private readonly List<(DrawData drawData, Player player, ShaderEntity shaderEntity, GlowmaskEntity glowmaskEntity)> _cache
			= new List<(DrawData, Player, ShaderEntity, GlowmaskEntity)>();

		/// <summary>
		/// WeaponOut will send us respective <see cref="DrawData"/>s for weapons being drawn
		/// For each entity on the item, the method will cache it along with the sent drawdata
		/// We return true so the DrawData is still added by WeaponOut,
		/// we later intercept the data's .Draw() call. See <see cref="DrawDataOnDraw"/>
		/// </summary>
		private bool CustomPreDraw(Player player, Item item, DrawData drawDrata)
		{
			var info = GraphicsGlobalItem.GetInfo(item);
			if (info.HasShaders)
			{
				for (int i = 0; i < info.ShaderEntities.Count; i++)
				{
					ShaderEntity entity = info.ShaderEntities[i];
					if (entity != null)
					{
						_cache.Add((drawDrata, player, entity, info.GlowmaskEntities[i]));
					}
				}
			}
			else if (info.HasGlowmasks)
			{
				foreach (var entity in info.GlowmaskEntities.Where(x => x != null))
				{
					_cache.Add((drawDrata, player, null, entity));
				}
			}
			return true;
		}
	}
}
