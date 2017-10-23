using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Effects;
using Loot.Modifiers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot
{
	//public class Vector3TagSerializer : TagSerializer<Vector3, TagCompound>
	//{
	//	public override TagCompound Serialize(Vector3 value) => new TagCompound
	//	{
	//		["x"] = value.X,
	//		["y"] = value.Y,
	//		["z"] = value.Z,
	//	};

	//	public override Vector3 Deserialize(TagCompound tag) => new Vector3(tag.GetFloat("x"), tag.GetFloat("y"), tag.GetFloat("z"));
	//}

	/// <summary>
	/// Holds entity based data
	/// </summary>
	class EMMItem : GlobalItem
	{
		public static EMMItem ItemInfo(Item item) => item.GetGlobalItem<EMMItem>();

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		public Modifier Modifier;

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			EMMItem clone = (EMMItem) base.Clone(item, itemClone);
			clone.Modifier = Modifier;
			return clone;
		}

		//public override void Load(Item item, TagCompound tag)
		//{
		//	Modifier = ModifierTagSerializer.SetFromTag(tag);
		//}

		//public override TagCompound Save(Item item)
		//	=> ModifierTagSerializer.ToTag(Modifier);

		//public override bool NeedsSaving(Item item)
		//{
		//	return Modifier != null;
		//}

		//public override void NetReceive(Item item, BinaryReader reader)
		//{
		//	int modLength = reader.Read();
		//	Modifier = reader.ReadBytes(modLength).FromByteArray<Modifier>();
		//}

		//public override void NetSend(Item item, BinaryWriter writer)
		//{
		//	var modBytes = Modifier.ToByteArray();
		//	writer.Write(modBytes.Length);
		//	writer.Write(modBytes);
		//}
	}

	/// <summary>
	/// Handles data globally
	/// </summary>
	class LootItem : GlobalItem
	{
		private EMMItem Info(Item item) => EMMItem.ItemInfo(item);
		private Modifier Modifier(Item item) => Info(item).Modifier;

		public override void UpdateEquip(Item item, Player player)
		{
			var modifier = Modifier(item);
			if (modifier == null)
			{
				Info(item).Modifier = new Modifier(Loader.GetEffect(0));
			}
			Info(item).Modifier.Apply(player, item, "UpdateEquip");
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			var modifier = Modifier(item);
			if (modifier != null)
			{
				int i = tooltips.Count;
				tooltips.Insert(i, new TooltipLine(mod, "Mod:Name", $"[{modifier.Rarity.Name}]") {overrideColor = modifier.Rarity.Color});
				tooltips.Insert(i + 1, new TooltipLine(mod, "Mod:Description", modifier.Description));
			}
		}
	}
}
