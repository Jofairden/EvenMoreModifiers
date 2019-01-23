using System;
using System.IO;
using Loot.Core.System.Core;
using Loot.Core.System.Loaders;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Core.System.Modifier
{
	/// <summary>
	/// Defines the rarity of a modifier
	/// </summary>
	public abstract class ModifierRarity : ILoadableContent, ILoadableContentSetter, ICloneable
	{
		public Mod Mod { get; internal set; }

		Mod ILoadableContentSetter.Mod
		{
			set => Mod = value;
		}

		public uint Type { get; internal set; }

		uint ILoadableContentSetter.Type
		{
			set => Type = value;
		}

		public string Name => GetType().Name;

		public virtual string RarityName => Name;
		public virtual Color? OverrideNameColor => null;
		public virtual string ItemPrefix => null;
		public virtual string ItemSuffix => null;

		public abstract float RequiredRarityLevel { get; }
		public abstract Color Color { get; }

		public virtual bool MatchesRequirements(ModifierPool modifierPool)
			=> modifierPool.MatchesRarity(this);

		public virtual void Clone(ref ModifierRarity clone)
		{
		}

		public object Clone()
		{
			ModifierRarity clone = (ModifierRarity) MemberwiseClone();
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
			string type = reader.ReadString();
			string modName = reader.ReadString();

			ModifierRarity r = ContentLoader.ModifierRarity.GetContent(modName, type);
			if (r == null)
			{
				throw new Exception($"ModifierRarity _NetReceive error for {modName}");
			}

			r.NetReceive(item, reader);
			return r;
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
			writer.Write(rarity.Name);
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
			if (MainLoader.Mods.TryGetValue(modname, out var assembly))
			{
				// If we load a null here, it means a rarity is unloaded
				ModifierRarity r = null;

				var saveVersion = tag.ContainsKey("ModifierRaritySaveVersion") ? tag.GetInt("ModifierRaritySaveVersion") : 1;

				string rarityTypeName = tag.GetString("Type");

				// adapt by save version
				if (saveVersion == 1)
				{
					// in first save version, modifiers were saved by full assembly namespace
					//m = (ModifierPool)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));// we modified saving
					rarityTypeName = rarityTypeName.Substring(rarityTypeName.LastIndexOf('.') + 1);
					r = ContentLoader.ModifierRarity.GetContent(modname, rarityTypeName);
				}
				else if (saveVersion == 2)
				{
					// from saveVersion 2 and onwards, they are saved by assembly (mod) and type name
					r = ContentLoader.ModifierRarity.GetContent(modname, rarityTypeName);
				}

				if (r != null)
				{
					//saveVersion 1, no longer needed.Type and Mod is already created by new instance
					//r.Type = tag.Get<uint>("RarityType");
					//r.Mod = ModLoader.GetMod(modname);
					r.Load(item, tag);
					return r;
				}

				return null;
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
				{"Type", rarity.GetType().Name},
				//{ "RarityType", rarity.Type },//Used to be saved in saveVersion 1
				{"ModName", rarity.Mod.Name},
				{"ModifierRaritySaveVersion", 2}
			};
			rarity.Save(item, tag);
			return tag;
		}
	}
}
