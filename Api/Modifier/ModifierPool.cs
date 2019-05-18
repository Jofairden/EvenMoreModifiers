using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Loot.Api.Attributes;
using Loot.Api.Content;
using Loot.Api.Ext;
using Loot.Api.Loaders;
using Loot.Modifiers;
using Loot.Rarities;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Api.Modifier
{
	/// <summary>
	/// Defines a modifier pool. A modifier pool holds a certain amount of effects in an array
	/// It allows to roll a 'themed' item if it hits an already defined pool
	/// Up to 4 effects are drawn from the pool, and activated
	/// </summary>
	public abstract class ModifierPool : ILoadableContent, ILoadableContentSetter, ICloneable
	{
		private const int SAVE_VERSION = 3;

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

		public IEnumerable<string> UniqueNames =>
			ActiveModifiers
				.Where(x => x.UniqueName != string.Empty)
				.Select(x => x.UniqueName);

		private Modifier[] _activeModifiers = new Modifier[0];
		public Modifier[] ActiveModifiers
		{
			get => _activeModifiers.Where(x => x.GetType() != typeof(NullModifier)).ToArray();
			protected internal set => _activeModifiers = value;
		}

		private PopulatePoolFromAttribute _populatePoolFrom;

		internal void CacheAttributes()
		{
			_populatePoolFrom = GetType().GetCustomAttribute<PopulatePoolFromAttribute>(true);
		}

		private IEnumerable<Modifier> _GetModifiers()
		{
			if (_populatePoolFrom != null)
			{
				var classes = _populatePoolFrom.GetClasses().SelectMany(x => x.Value);

				foreach (var @class in classes)
				{
					var m = ContentLoader.Modifier.GetContent(@class);
					if (m != null)
					{
						yield return m;
					}
				}
			}

			foreach (Modifier m in GetModifiers())
			{
				if (m != null)
				{
					yield return m;
				}
			}
		}

		public virtual IEnumerable<Modifier> GetModifiers()
			=> Enumerable.Empty<Modifier>();

		/// <summary>
		/// Returns an enumerable of the rollable modifiers in the given context
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		protected internal IEnumerable<Modifier> GetRollableModifiers(ModifierContext ctx)
			=> _GetModifiers().Where(x => x._CanRoll(ctx));

		/// <summary>
		/// Returns the sum of the rarity levels of the active modifiers
		/// </summary>
		public float TotalRarityLevel
			=> ActiveModifiers.Select(m => m.Properties.RarityLevel).DefaultIfEmpty(0).Sum();

		/// <summary>
		/// Returns an enumerable of the tooltiplines of the active modifiers
		/// </summary>
		public IEnumerable<ModifierTooltipLine[]> Description
			=> ActiveModifiers.Select(m => m.GetTooltip().Build().ToArray());

		public virtual float RollChance => 1f;

		//internal float ModifierRollChance(int len) => 0.5f / (float)Math.Pow(2, len);

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
			=> ctx.Item?.IsModifierRollableItem() ?? true;

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
			clone.ActiveModifiers =
				ActiveModifiers?
					.Select(x => (Modifier)x?.Clone() ?? Loot.Instance.GetNullModifier())
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
			string type = reader.ReadString();
			string modName = reader.ReadString();

			ModifierPool p = ContentLoader.ModifierPool.GetContent(modName, type);
			if (p == null)
			{
				throw new Exception($"Modifier _NetReceive error for {modName}");
			}

			int activeModifiersSize = reader.ReadInt32();
			var list = new List<Modifier>();
			for (int i = 0; i < activeModifiersSize; ++i)
			{
				list.Add(Modifier._NetReceive(item, reader));
			}

			p.ActiveModifiers = list.ToArray();
			p.NetReceive(item, reader);
			return p;
		}

		/// <summary>
		/// Allows modder to do custom NetSend here
		/// </summary>
		public virtual void NetSend(Item item, BinaryWriter writer)
		{
		}

		protected internal static void _NetSend(ModifierPool modifierPool, Item item, BinaryWriter writer)
		{
			writer.Write(modifierPool.Name);
			writer.Write(modifierPool.Mod.Name);

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

			string modName = tag.GetString("ModName");

			if (RegistryLoader.Mods.TryGetValue(modName, out var assembly))
			{
				// If we manage to load null here, that means some pool got unloaded
				ModifierPool p = null;
				var saveVersion = tag.ContainsKey("ModifierPoolSaveVersion") ? tag.GetInt("ModifierPoolSaveVersion") : 1;

				string poolTypeName = tag.GetString("Type");

				// adapt by save version
				if (saveVersion == 1)
				{
					// in first save version, modifiers were saved by full assembly namespace
					//m = (ModifierPool)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));// we modified saving
					poolTypeName = poolTypeName.Substring(poolTypeName.LastIndexOf('.') + 1);
					p = ContentLoader.ModifierPool.GetContent(modName, poolTypeName);
				}
				else if (saveVersion >= 2)
				{
					// from saveVersion 2 and onwards, they are saved by assembly (mod) and type name
					p = ContentLoader.ModifierPool.GetContent(modName, poolTypeName);
				}

				// if we have a pool
				if (p != null)
				{
					// saveVersion 1, no longer needed. Type and Mod is already created by new instance
					//m.Type = tag.Get<uint>("ModifierType");
					// m.Mod = ModLoader.GetMod(modname);
					// preload rarity
					if (saveVersion < 3)
					{
						// Since save version 3, the rarity is no longer saved on the pool, rather on the item itself
						// however, it was saved on the pool in earlier versions, so we need to catch it here.
						ModifierRarity preloadRarity = ModifierRarity._Load(item, tag.GetCompound("Rarity"));

						LootModItem.GetInfo(item).ModifierRarity =
							preloadRarity ?? ContentLoader.ModifierRarity.GetContent(typeof(CommonRarity));
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

						p.ActiveModifiers = list.ToArray();
					}

					p.Load(item, tag);
					return p;
				}

				return null;
			}

			Loot.Logger.ErrorFormat("There was a load error for modifierpool, TC: {0}", tag);
			return null;
		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(Item, TagCompound)"/>
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
				{"ModifierPoolSaveVersion", SAVE_VERSION}, // increments each time save is changed
				{"ActiveModifiers", modifierPool.ActiveModifiers.Length}
			};

			for (int i = 0; i < modifierPool.ActiveModifiers.Length; ++i)
			{
				tag.Add($"ActiveModifier{i}", Modifier.Save(item, modifierPool.ActiveModifiers[i]));
			}

			modifierPool.Save(item, tag);
			return tag;
		}
	}
}
