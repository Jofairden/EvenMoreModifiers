using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Api.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
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
			On.Terraria.DataStructures.DrawData.Draw += DrawDataOnDraw;
			mod.Call("AddCustomPreDrawMethod", (Func<Player, Item, DrawData, bool>)CustomPreDraw);
		}

		private void DrawDataOnDraw(On.Terraria.DataStructures.DrawData.orig_Draw orig, ref DrawData self, SpriteBatch sb)
		{
			bool found = false;
			foreach ((DrawData drawData, Item item, ShaderEntity shaderEntity, GlowmaskEntity glowmaskEntity) in _drawDatas)
			{
//				if (item.type == Terraria.ID.ItemID.GrenadeLauncher)
//				{
//					bool v = true;
//				}
				if (IsSameDrawData(drawData, self))
				{
					found = true;
					if (glowmaskEntity != null)
					{
						glowmaskEntity.SkipUpdatingDrawData = true;
						// glowmaskEntity.DrawData = drawData;
						glowmaskEntity.SetIdentity(item);
					}

					if (shaderEntity != null)
					{
						shaderEntity.SkipUpdatingDrawData = true;
						shaderEntity.DrawData = drawData;
						shaderEntity.SetIdentity(item);
						shaderEntity.DoDrawLayeredEntity(sb, drawData.color, drawData.color, drawData.scale.LengthSquared(), drawData.rotation, glowmaskEntity);
					}
					_drawDatas.RemoveAll(x => IsSameDrawData(x.Item1, drawData));
					break;
				}
			}
			if (!found)
			{
				orig(ref self, sb);
			}
		}

		private readonly List<(DrawData, Item, ShaderEntity, GlowmaskEntity)> _drawDatas = new List<(DrawData, Item, ShaderEntity, GlowmaskEntity)>();

		// I HATE STRUCTS!!!!!!!!!!!!!!!!!!
		private bool IsSameDrawData(DrawData first, DrawData second)
		{
			// TODO doesn't work for shit with mounts.......
			(Color, Rectangle, Vector2, Vector2) firstDecon = (first.color, first.destinationRectangle, first.origin, first.position);
			(Color, Rectangle, Vector2, Vector2) secondDecon = (second.color, second.destinationRectangle, second.origin, second.position);
			return Equals(firstDecon, secondDecon);
		}

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
						_drawDatas.Add((drawDrata, item, entity, info.GlowmaskEntities[i]));
					}
				}
			}
			else if (info.HasGlowmasks)
			{
				// TODO
			}
			return true;
		}
	}
}
