using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers
{
	//public class ModifierTagSerializer : TagSerializer<Modifier, TagCompound>
	//{
	//	public static Modifier SetFromTag(TagCompound tag)
	//	{
	//		Modifier mod = new Modifier(tag.GetString("name"), tag.GetString("description"),
	//			tag.Get<ModifierApplyDelegate>("_applyDelegate"), tag.GetAsInt("strength"));
	//		mod.ApplyRarity(tag.Get<ModifierRarity>("rarity"));
	//		return mod;
	//	}

	//	public static TagCompound ToTag(Modifier mod)
	//	{
	//		return new TagCompound
	//		{
	//			["name"] = mod.Name,
	//			["description"] = mod.Description,
	//			["_applyDelegate"] = mod.GetApplyDelegate(),
	//			["strength"] = mod.Strength,
	//			["rarity"] = mod.Rarity
	//		};
	//	}

	//	public override TagCompound Serialize(Modifier value)
	//		=> ToTag(value);

	//	public override Modifier Deserialize(TagCompound tag)
	//		=> SetFromTag(tag);
	//}
}
