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
		/// <para>If the DrawData was cached, it came from WeaponOut and we do not call the original draw code</para>
		/// <para>Instead, we call our custom draw code on the cached entities</para>
		/// </summary>
		private void DrawDataOnDraw(On.Terraria.DataStructures.DrawData.orig_Draw orig, ref DrawData self, SpriteBatch sb)
		{
			var data = self;
			var cachedDatas = _cache.Where(x => x.Item1.texture == data.texture).ToList();
			if (cachedDatas.Any())
			{
				foreach (var cachedData in cachedDatas)
				{
					var shaderEntity = cachedData.Item3;
					var glowmaskEntity = cachedData.Item4;
					var plr = cachedData.Item2;
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
			}
			else
			{
				orig(ref self, sb);
			}
		}

		private Color? _lightColor;
		private readonly List<(DrawData, Player, ShaderEntity, GlowmaskEntity)> _cache = new List<(DrawData, Player, ShaderEntity, GlowmaskEntity)>();

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
