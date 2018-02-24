using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers
{
	/// <summary>
	/// Defines the rarity of a modifier
	/// </summary>
	public abstract class ModifierRarity : ICloneable/*, TagSerializable*/
	{
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }

		public virtual string Name => GetType().Name;
		public virtual Color? OverrideNameColor => null;
		public virtual string ItemPrefix => null;
		public virtual string ItemSuffix => null;

		public abstract float RequiredRarityLevel { get; }
		public abstract Color Color { get; }

		public virtual bool MatchesRequirements(Modifier modifier)
			=> modifier.MatchesRarity(this);

		public override string ToString()
			=> EMMUtils.JSLog(typeof(ModifierRarity), this);

		public virtual void Clone(ref ModifierRarity clone)
		{

		}

		public object Clone()
		{
			ModifierRarity clone = (ModifierRarity)MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			Clone(ref clone);
			return clone;
		}

		//public static Func<TagCompound, ModifierRarity> DESERIALIZER = tag =>
		//{
		//	TagSerializer serializer;
		//	if (TagSerializer.TryGetSerializer(typeof(ModifierRarity), out serializer))
		//	{
		//		return (ModifierRarity)serializer.Deserialize(tag);
		//	}
		//	throw new Exception("DESERIALIZER for ModifierRarity error");
		//};

		//public TagCompound SerializeData()
		//{
		//	TagSerializer serializer;
		//	if (TagSerializer.TryGetSerializer(GetType(), out serializer))
		//	{
		//		return (TagCompound)serializer.Serialize(this);
		//	}
		//	throw new Exception("SerializeData() for ModifierRarity error");
		//}
	}
}
