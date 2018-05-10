using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Core
{
	/// <summary>
	/// Defines the rarity of a modifier
	/// </summary>
	public abstract class ModifierRarity : ICloneable
	{
		public Mod Mod { get; internal set; }
		public uint Type { get; internal set; }

		public virtual string Name => GetType().Name;
		public virtual Color? OverrideNameColor => null;
		public virtual string ItemPrefix => null;
		public virtual string ItemSuffix => null;

		public abstract float RequiredRarityLevel { get; }
		public abstract Color Color { get; }

		public virtual bool MatchesRequirements(ModifierPool modifierPool)
			=> modifierPool.MatchesRarity(this);

		public override string ToString()
			=> EMMUtils.JSLog(typeof(ModifierRarity), this);

		/// <summary>
		/// Returns the ModifierRarity specified by type, null if not present
		/// </summary>
		public static ModifierRarity GetModifierRarity(ushort type)
			=> EMMLoader.GetModifierRarity(type);

		public ModifierRarity AsNewInstance()
			=> (ModifierRarity)Activator.CreateInstance(GetType());

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

		/// <summary>
		/// Allows the modder to do custom NetReceive
		/// </summary>
		/// <param name="item"></param>
		/// <param name="reader"></param>
		public virtual void NetReceive(Item item, BinaryReader reader)
		{
		}

		protected internal static ModifierRarity _NetReceive(Item item, BinaryReader reader)
		{
			string Type = reader.ReadString();
			uint RarityType = reader.ReadUInt32();
			string ModName = reader.ReadString();

			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(ModName, out assembly))
			{
				ModifierRarity r = (ModifierRarity)Activator.CreateInstance(assembly.GetType(Type));
				r.Type = RarityType;
				r.Mod = ModLoader.GetMod(ModName);
				r.NetReceive(item, reader);
				return r;
			}
			throw new Exception($"ModifierRarity _NetReceive error for {ModName}");
		}

		/// <summary>
		/// Allows modder to do custom NetSend here
		/// </summary>
		/// <param name="item"></param>
		/// <param name="writer"></param>
		public virtual void NetSend(Item item, BinaryWriter writer)
		{
		}

		protected internal static void _NetSend(ModifierRarity rarity, Item item, BinaryWriter writer)
		{
			writer.Write(rarity.GetType().FullName);
			writer.Write(rarity.Type);
			writer.Write(rarity.Mod.Name);
		}

		/// <summary>
		/// Allows modder to do custom loading here
		/// Use the given TC to pull data you saved using <see cref="Save(Item,TagCompound)"/>
		/// </summary>
		public virtual void Load(Item item, TagCompound tag)
		{
		}

		protected internal static ModifierRarity _Load(Item item, TagCompound tag)
		{
			string modname = tag.GetString("ModName");
			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(modname, out assembly))
			{
				// If we load a null here, it means a rarity is unloaded
				ModifierRarity r;
				try
				{
					r = (ModifierRarity)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));
				}
				catch (Exception)
				{
					return null;
				}

				r.Type = tag.Get<uint>("RarityType");
				r.Mod = ModLoader.GetMod(modname);
				r.Load(item, tag);
				return r;
			}
			throw new Exception($"ModifierRarity load error for {modname}");
		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(Item,TagCompound)"/>
		/// </summary>
		public virtual void Save(Item item, TagCompound tag)
		{
		}

		protected internal static TagCompound Save(Item item, ModifierRarity rarity)
		{
			var tag = new TagCompound
			{
				{ "Type", rarity.GetType().FullName },
				{ "RarityType", rarity.Type },
				{ "ModName", rarity.Mod.Name },
				{ "ModifierRaritySaveVersion", 1 }
			};
			rarity.Save(item, tag);
			return tag;
		}
	}
}
