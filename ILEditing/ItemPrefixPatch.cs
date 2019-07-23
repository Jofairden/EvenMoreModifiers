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
			bool ret = retCursor.TryGotoNext(MoveType.After, i => i.OpCode == OpCodes.Ret);
			if (!ret || retCursor.Next == null)
			{
				// we didn't find the ret instruction or we ended at end of function (debug)
				// in this case, look for the br instruction that branches to ret
				retCursor = cursor.Clone(); // refresh
				retCursor.TryGotoNext(MoveType.After, i => i.OpCode == OpCodes.Br);
				// after this we should be inbetween the br and ldloc0 instructions
				// we can now emit our new label after the br instruction to move past the branching
			}
			// If our emitted delegate holds true, transfer control to our label
			cursor.Emit(OpCodes.Brtrue, retCursor.MarkLabel());
			// We essentially "goto" our new label is the item is armor, skipping the return or branching
		}

		private bool IsArmor(Item item) => item.IsArmor();
	}
}
