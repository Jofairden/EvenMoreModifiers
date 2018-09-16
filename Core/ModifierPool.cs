using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Core
{
	/// <summary>
	/// Defines a modifier pool. A modifier pool holds a certain amount of effects in an array
	/// It allows to roll a 'themed' item if it hits an already defined pool
	/// Up to 4 effects are drawn from the pool, and activated
	/// </summary>
	public abstract class ModifierPool : ICloneable
	{
		public uint Type { get; protected internal set; }
		public Mod Mod { get; protected internal set; }
		public ModifierRarity Rarity { get; protected internal set; }
		protected internal Modifier[] Modifiers;
		public Modifier[] ActiveModifiers { get; protected internal set; }

		/// <summary>
		/// Returns an enumerable of the rollable modifiers in the given context
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		protected internal IEnumerable<Modifier> RollableModifiers(ModifierContext ctx)
			=> Modifiers.Where(x => x._CanRoll(ctx));

		/// <summary>
		/// Returns the sum of the rarity levels of the active modifiers
		/// </summary>
		public float TotalRarityLevel
			=> ActiveModifiers.Select(m => m.Properties.RarityLevel).DefaultIfEmpty(0).Sum();

		/// <summary>
		/// Returns an enumerable of the tooltiplines of the active modifiers
		/// </summary>
		public IEnumerable<ModifierTooltipLine[]> Description
			=> ActiveModifiers.Select(m => m.TooltipLines);

		public virtual string Name => GetType().Name;
		public virtual float RollChance => 1f;

		/// <summary>
		/// Returns the ModifierPool specified by type, null if not present
		/// </summary>
		public static ModifierPool GetModifierPool(ushort type)
			=> EMMLoader.GetModifierPool(type);

		public ModifierPool AsNewInstance()
			=> (ModifierPool)Activator.CreateInstance(GetType());

		/// <summary>
		/// Gets the next appropriate rarit for this pool and applies it and returns it.
		/// </summary>
		/// <returns></returns>
		internal ModifierRarity UpdateRarity()
		{
			Rarity = EMMLoader.GetPoolRarity(this);
			return Rarity;
		}

		//internal float ModifierRollChance(int len) => 0.5f / (float)Math.Pow(2, len);

		internal static bool IsValidFor(Item item)
			=> item.IsModifierRollableItem();

		/// <summary>
		/// Returns if this pool can roll in the given context. 
		/// By default returns if any modifier's <see cref="Modifier._CanRoll"/> returns true, and this pool's <see cref="CanRoll"/> returns true
		/// </summary>
		protected internal bool _CanRoll(ModifierContext ctx)
			=> CanRoll(ctx);

		/// <summary>
		/// Returns if this pool can roll in the given context
		/// Returns true by default
		/// </summary>
		public virtual bool CanRoll(ModifierContext ctx)
			=> true;

		/// <summary>
		/// By default returns if this pool matches the given <see cref="ModifierRarity"/>'s <see cref="ModifierRarity.RequiredRarityLevel"/>
		/// </summary>
		public virtual bool MatchesRarity(ModifierRarity rarity)
			=> TotalRarityLevel >= rarity.RequiredRarityLevel;

		/// <summary>
		/// Will run <see cref="Modifier.Apply"/> for all modifiers in <see cref="ActiveModifiers"/>
		/// </summary>
		/// <param name="item">The item to apply to, which is passed to <see cref="Modifier.Apply"/></param>
		internal void ApplyModifiers(Item item)
		{
			foreach (Modifier m in ActiveModifiers)
			{
				m.Apply(item);
			}
		}

		/// <summary>
		/// Allows modders to do custom cloning
		/// Happens after default cloning
		/// </summary>
		public virtual void Clone(ref ModifierPool clone)
		{
		}

		public object Clone()
		{
			ModifierPool clone = (ModifierPool)MemberwiseClone();
			clone.Type = Type;
			clone.Mod = Mod;
			clone.Rarity = (ModifierRarity)Rarity?.Clone();
			clone.Modifiers =
				Modifiers?
					.Select(x => x?.Clone())
					.Cast<Modifier>()
					.ToArray();
			clone.ActiveModifiers =
				ActiveModifiers?
					.Select(x => x?.Clone())
					.Cast<Modifier>()
					.ToArray();
			Clone(ref clone);
			return clone;
		}

		/// <summary>
		/// Allows the modder to do custom NetReceive
		/// </summary>
		public virtual void NetReceive(Item item, BinaryReader reader)
		{
		}

		protected internal static ModifierPool _NetReceive(Item item, BinaryReader reader)
		{
			string Type = reader.ReadString();
			uint ModifierType = reader.ReadUInt32();
			string ModName = reader.ReadString();
			ModifierRarity ModifierRarity = ModifierRarity._NetReceive(item, reader);
			int ActiveModifiersSize = reader.ReadInt32();
			var list = new List<Modifier>();
			for (int i = 0; i < ActiveModifiersSize; ++i)
			{
				list.Add(Modifier._NetReceive(item, reader));
			}

			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(ModName, out assembly))
			{
				ModifierPool m = (ModifierPool)Activator.CreateInstance(assembly.GetType(Type));
				m.Type = ModifierType;
				m.Mod = ModLoader.GetMod(ModName);
				m.Rarity = ModifierRarity;
				m.ActiveModifiers = list.ToArray();
				m.NetReceive(item, reader);
				return m;
			}

			throw new Exception($"Modifier _NetReceive error for {ModName}");
		}

		/// <summary>
		/// Allows modder to do custom NetSend here
		/// </summary>
		public virtual void NetSend(Item item, BinaryWriter writer)
		{
		}

		protected internal static void _NetSend(ModifierPool modifierPool, Item item, BinaryWriter writer)
		{
			writer.Write(modifierPool.GetType().FullName);
			writer.Write(modifierPool.Type);
			writer.Write(modifierPool.Mod.Name);

			ModifierRarity._NetSend(modifierPool.Rarity, item, writer);

			writer.Write(modifierPool.ActiveModifiers.Length);
			for (int i = 0; i < modifierPool.ActiveModifiers.Length; ++i)
			{
				Modifier._NetSend(modifierPool.ActiveModifiers[i], item, writer);
			}

			modifierPool.NetSend(item, writer);
		}

		/// <summary>
		/// Allows modder to do custom loading here
		/// Use the given TC to pull data you saved using <see cref="Save(Item,TagCompound)"/>
		/// </summary>
		public virtual void Load(Item item, TagCompound tag)
		{
		}

		protected internal static ModifierPool _Load(Item item, TagCompound tag)
		{
			if (tag == null || tag.ContainsKey("EMMErr:PoolNullErr"))
			{
				return null;
			}

			string modname = tag.GetString("ModName");
			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(modname, out assembly))
			{
				// If we manage to load null here, that means some pool got unloaded
				ModifierPool m = null;
				var saveVersion = tag.ContainsKey("ModifierPoolSaveVersion") ? tag.GetInt("ModifierPoolSaveVersion") : 1;

				string poolTypeName = tag.GetString("Type");

				// adapt by save version
				if (saveVersion == 1)
				{
					// in first save version, modifiers were saved by full assembly namespace
					//m = (ModifierPool)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));// we modified saving
					poolTypeName = poolTypeName.Substring(poolTypeName.LastIndexOf('.') + 1);
					m = EMMLoader.GetLoadPreparedModifierPool(modname, poolTypeName);
				}
				else if (saveVersion == 2)
				{
					// from saveVersion 2 and onwards, they are saved by assembly (mod) and type name
					m = EMMLoader.GetLoadPreparedModifierPool(modname, poolTypeName);
				}

				// if we have a pool
				if (m != null)
				{
					// saveVersion 1, no longer needed. Type and Mod is already created by new instance
					//m.Type = tag.Get<uint>("ModifierType");
					// m.Mod = ModLoader.GetMod(modname);
					// preload rarity
					ModifierRarity preloadRarity = ModifierRarity._Load(item, tag.Get<TagCompound>("Rarity"));
					bool rarityUnloaded = preloadRarity == null;
					if (!rarityUnloaded)
					{
						m.Rarity = preloadRarity;
					}

					int activeModifiers = tag.GetAsInt("ActiveModifiers");
					if (activeModifiers > 0)
					{
						var list = new List<Modifier>();
						for (int i = 0; i < activeModifiers; ++i)
						{
							// preload to take unloaded modifiers into account
							var loaded = Modifier._Load(item, tag.Get<TagCompound>($"ActiveModifier{i}"));
							if (loaded != null)
							{
								list.Add(loaded);
							}
						}

						m.ActiveModifiers = list.ToArray();
					}

					m.Load(item, tag);

					// If our rarity was unloaded, attempt rolling a new one that is applicable
					if (rarityUnloaded && m.ActiveModifiers != null && m.ActiveModifiers.Length > 0)
					{
						m.Rarity = EMMLoader.GetPoolRarity(m);
					}

					m.Modifiers = null;
					return m;
				}

				return null;
			}

			throw new Exception($"Modifier load error for {modname}");
		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(TagCompound)"/>
		/// </summary>
		/// <param name="tag"></param>
		public virtual void Save(Item item, TagCompound tag)
		{
		}

		protected internal static TagCompound Save(Item item, ModifierPool modifierPool)
		{
			if (modifierPool == null)
			{
				return new TagCompound { { "EMMErr:PoolNullErr", "ModifierPool was null err" } };
			}

			var tag = new TagCompound
			{
				{"Type", modifierPool.GetType().Name},
				//{ "ModifierType", modifierPool.Type }, //Used to be saved in saveVersion 1
				{"ModName", modifierPool.Mod.Name},
				{"Rarity", ModifierRarity.Save(item, modifierPool.Rarity)},
				{"ModifierPoolSaveVersion", 2} // increments each time save is changed
			};
			tag.Add("ActiveModifiers", modifierPool.ActiveModifiers.Length);
			for (int i = 0; i < modifierPool.ActiveModifiers.Length; ++i)
			{
				tag.Add($"ActiveModifier{i}", Modifier.Save(item, modifierPool.ActiveModifiers[i]));
			}

			modifierPool.Save(item, tag);
			return tag;
		}

		public override string ToString()
			=> EMMUtils.JSLog(typeof(ModifierPool), this);
	}
}
