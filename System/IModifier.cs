using Terraria;
using Terraria.ModLoader.IO;

namespace Loot.System
{
	interface IModifier
	{
		ModifierProperties GetModifierProperties(Item item);
		bool CanRoll(ModifierContext ctx);
		bool CanRollCraft(ModifierContext ctx);
		bool CanRollPickup(ModifierContext ctx);
		bool CanRollReforge(ModifierContext ctx);
		bool UniqueRoll(ModifierContext ctx);
		void Roll(Item item);
		void Apply(Item item);
		void Clone(ref Modifier clone);
		void Load(TagCompound tag);
		void Save(TagCompound tag);
	}
}
