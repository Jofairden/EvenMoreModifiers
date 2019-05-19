using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Api.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Main = On.Terraria.Main;

namespace Loot.ModSupport
{
	internal class WeaponOutSupport : ModSupport
	{
		public override string ModName => "WeaponOut";

		public override bool CheckValidity(Mod mod) => mod.Version >= new Version(1, 6, 4);

		public override void AddClientSupport(Mod mod)
		{
			Main.DrawPlayer += MainOnDrawPlayer;
			On.Terraria.DataStructures.DrawData.Draw += DrawDataOnDraw;
			mod.Call("AddCustomPreDrawMethod", (Func<Player, Item, DrawData, bool>)CustomPreDraw);
		}

		private void MainOnDrawPlayer(Main.orig_DrawPlayer orig, Terraria.Main self, Player drawplayer, Vector2 position, float rotation, Vector2 rotationorigin, float shadow)
		{
			_cache.Clear();
			orig(self, drawplayer, position, rotation, rotationorigin, shadow);
		}

		private void DrawDataOnDraw(On.Terraria.DataStructures.DrawData.orig_Draw orig, ref DrawData self, SpriteBatch sb)
		{
			var data = self;
			var cachedData = _cache.FirstOrDefault(x => x.Item1.texture == data.texture);
			if (!cachedData.Equals(default))
			{
				var shaderEntity = cachedData.Item3;
				var glowmaskEntity = cachedData.Item4;
				if (shaderEntity != null)
				{
					shaderEntity.SkipUpdatingDrawData = true;
					shaderEntity.DrawData = self;
					shaderEntity.SetIdentity(cachedData.Item2);
					shaderEntity.DoDrawLayeredEntity(sb, self.color, self.color, self.scale.LengthSquared(), self.rotation, glowmaskEntity);
				} 
				else if (glowmaskEntity != null)
				{
					glowmaskEntity.SkipUpdatingDrawData = true;
					glowmaskEntity.DrawData = self;
					glowmaskEntity.SetIdentity(cachedData.Item2);
					glowmaskEntity.DoDrawGlowmask(sb, self.color, self.color, self.rotation, self.scale.LengthSquared(), glowmaskEntity.Entity.whoAmI);
				}
			}
			else
			{
				orig(ref self, sb);
			}
		}

		private readonly List<(DrawData, Item, ShaderEntity, GlowmaskEntity)> _cache = new List<(DrawData, Item, ShaderEntity, GlowmaskEntity)>();

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
						_cache.Add((drawDrata, item, entity, info.GlowmaskEntities[i]));
					}
				}
			}
			else if (info.HasGlowmasks)
			{
				foreach (var entity in info.GlowmaskEntities.Where(x => x != null))
				{
					_cache.Add((drawDrata, item, null, entity));
				}
			}
			return true;
		}
	}
}
