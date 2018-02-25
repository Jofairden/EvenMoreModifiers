using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Modifiers
{
	public struct ModifierEffectTooltipLine
	{
		public string Text;
		public Color? Color;
	}

	/// <summary>
	/// Defines a modifier effect
	/// </summary>
	public abstract class ModifierEffect : ICloneable, TagSerializable
	{
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }
		public float Magnitude { get; internal set; } = 1f;
		public float Power { get; internal set; } = 1f;

		public virtual string Name => GetType().Name;
		public virtual float BasePower => 1f;
		public virtual float MinMagnitude => 1f;
		public virtual float MaxMagnitude => 1f;
		public virtual float RarityLevel => 1f;
		public virtual float RollChance => 1f;

		public virtual ModifierEffectTooltipLine[] Description { get; }

		internal float RollAndApplyMagnitude()
		{
			Magnitude = MinMagnitude + Main.rand.NextFloat() * (MaxMagnitude - MinMagnitude);
			Power = BasePower * Magnitude;
			return Power;
		}

		public virtual bool CanRoll(ModifierContext ctx) => true;

		public virtual void UpdateItem(ModifierContext ctx, bool equipped = false)
		{

		}

		public virtual void ApplyItem(ModifierContext ctx)
		{

		}

		public virtual void ApplyNPC(ModifierContext ctx)
		{

		}

		public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{

		}

		public virtual void Clone(ref ModifierEffect clone)
		{

		}

		public object Clone()
		{
			ModifierEffect clone = (ModifierEffect)MemberwiseClone();
			clone.Mod = Mod;
			clone.Type = Type;
			clone.Magnitude = Magnitude;
			clone.Power = Power;
			Clone(ref clone);
			return clone;
		}

		//public static TagCompound Save(ModifierEffect effect)
		//{
		//	return new TagCompound {
		//			{ "Type", effect.GetType() },
		//			{ "EffectType", effect.Type },
		//			{ "ModName", effect.Mod.Name },
		//			{ "Magnitude", effect.Magnitude },
		//			{ "Power", effect.Power  },
		//		}; ;
		//}

		protected internal static ModifierEffect Load(TagCompound tag)
		{
			string modname = tag.GetString("ModName");
			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(modname, out assembly))
			{
				ModifierEffect e = (ModifierEffect)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));
				e.Type = tag.Get<uint>("EffectType");
				e.Mod = ModLoader.GetMod(modname);
				e.Magnitude = tag.GetFloat("Magnitude");
				e.Power = tag.GetFloat("Power");
				return e;
			}
			throw new Exception($"ModifierEffect load error for {modname}");
		}

		public static Func<TagCompound, ModifierEffect> DESERIALIZER = tag => Load(tag);

		public TagCompound SerializeData()
		{
			var effect = this;
			var tag = new TagCompound {
					{ "Type", effect.GetType().FullName },
					{ "EffectType", effect.Type },
					{ "ModName", effect.Mod.Name },
					{ "Magnitude", effect.Magnitude },
					{ "Power", effect.Power  },
				}; ;
			return tag;
		}

		//public static Func<TagCompound, ModifierEffect> DESERIALIZER = tag =>
		//{
		//	TagSerializer serializer;
		//	if (TagSerializer.TryGetSerializer(typeof(ModifierEffect), out serializer))
		//	{
		//		return (ModifierEffect)serializer.Deserialize(tag);
		//	}
		//	throw new Exception("DESERIALIZER for ModifierEffect error");
		//};

		//public TagCompound SerializeData()
		//{
		//	TagSerializer serializer;
		//	if (TagSerializer.TryGetSerializer(GetType(), out serializer))
		//	{
		//		return (TagCompound)serializer.Serialize(this);
		//	}
		//	throw new Exception("SerializeData() for ModifierEffect error");
		//}
	}
}
