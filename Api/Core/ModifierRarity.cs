using System;
using System.IO;
using Loot.Api.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Api.Core
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

		/// <summary>
		/// Describes the chance of this rarity upgrading to the next tier
		/// If null, is the same as 0f (no chance)
		/// </summary>
		public virtual float? UpgradeChance => null;

		/// <summary>
		/// Describes the chance of this rarity downgrading to the previous tier
		/// If null, is the same as 0f (no chance)
		/// </summary>
		public virtual float? DowngradeChance => null;

		public virtual float ExtraMagnitudePower => 0f;
		public virtual float ExtraLuck => 0f;
		public virtual Type Downgrade => null;
		public virtual Type Upgrade => null;

		public abstract Color Color { get; }

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

		/// <summary>
		/// Allows modder to do custom NetSend here
		/// </summary>
		/// <param name="item"></param>
		/// <param name="writer"></param>
		public virtual void NetSend(Item item, BinaryWriter writer)
		{
		}

		/// <summary>
		/// Allows modder to do custom loading here
		/// Use the given TagCompound to pull data you saved using <see cref="Save(Item,TagCompound)"/>
		/// </summary>
		public virtual void Load(Item item, TagCompound tag)
		{
		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(Item,TagCompound)"/>
		/// </summary>
		public virtual void Save(Item item, TagCompound tag)
		{
		}
	}
}
