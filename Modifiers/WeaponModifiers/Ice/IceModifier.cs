using Loot.Api.Graphics;
using Loot.Modifiers.Base;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Loot.Modifiers.WeaponModifiers.Ice
{
	public abstract class IceModifier : WeaponModifier
	{
		public override ShaderEntity GetShaderEntity(Item item)
		{
			return new ShaderEntity(item,
				GameShaders.Armor.GetShaderIdFromItemId(ItemID.HadesDye),
				drawLayer: ShaderDrawLayer.Front,
				shaderDrawColor: Color.AliceBlue
			);
		}
	}
}
