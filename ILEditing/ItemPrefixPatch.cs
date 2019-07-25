using System;
using Loot.Api.Ext;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;

namespace Loot.ILEditing
{
	/// <summary>
	/// This patch allows armor items to be reforged by the tinkerer
	/// </summary>
	internal class ItemPrefixPatch : ILEdit
	{
		public override void Apply(bool dedServ)
		{
			IL.Terraria.Item.Prefix += ItemOnPrefix;
		}

		private void ItemOnPrefix(ILContext il)
		{
			var cursor = new ILCursor(il);

			// Find !item.accessory check
			if (!cursor.TryGotoNext(MoveType.Before,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld("Terraria.Item", "accessory")))
				return;

			// Append our check
			cursor.Emit(OpCodes.Ldarg_0); // emit item
			cursor.EmitDelegate<Func<Item, bool>>(IsArmor);

			// Clone cursor
			var retCursor = cursor.Clone();
			// Go after the vanilla false return statement
			// we may find either a return op or a br (branching) op for debug
			// in debug there is one ret op at the end of method, with br ops branching to it
			// so we can match either one and we will be in the right position.
			retCursor.TryGotoNext(MoveType.After, i => i.OpCode == OpCodes.Ret || i.OpCode == OpCodes.Br);

			// If our emitted delegate holds true, transfer control to our label
			cursor.Emit(OpCodes.Brtrue, retCursor.MarkLabel());
			// We essentially "goto" our new label is the item is armor, skipping the return or branching
		}

		private bool IsArmor(Item item) => item.IsArmor();
	}
}
