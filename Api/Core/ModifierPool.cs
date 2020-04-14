using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Loot.Api.Attributes;
using Loot.Api.Content;
using Loot.Api.Ext;
using Loot.Api.Loaders;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Loot.Api.Core
{
	[DoNotLoad]
	public class FiniteModifierPool : ModifierPool
	{
		public readonly List<Modifier> Modifiers;

		public FiniteModifierPool(List<Modifier> modifiers)
		{
			Modifiers = modifiers ?? new List<Modifier>();
		}

		public void Apply(Item item)
		{
			Modifiers.ForEach(mod => mod.Apply(item));
		}
	}

	/// <summary>
	/// Defines a modifier pool. A modifier pool holds a certain amount of effects in an array
	/// It allows to roll a 'themed' item if it hits an already defined pool
	/// Up to 4 effects are drawn from the pool, and activated
	/// </summary>
	public abstract class ModifierPool : ILoadableContent, ILoadableContentSetter, ICloneable
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

		///// <summary>
		///// Returns the sum of the rarity levels of the active modifiers
		///// </summary>
		//public float TotalRarityLevel
		//	=> ActiveModifiers.Select(m => m.Properties.RarityLevel).DefaultIfEmpty(0).Sum();

		///// <summary>
		///// Returns an enumerable of the tooltiplines of the active modifiers
		///// </summary>
		//public IEnumerable<ModifierTooltipLine[]> Description
		//	=> ActiveModifiers.Select(m => m.GetTooltip().Build().ToArray());

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
			//foreach (Modifier m in ActiveModifiers)
			//{
			//	m.Apply(item);
			//}
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
			//clone.ActiveModifiers =
			//	ActiveModifiers?
			//		.Select(x => (Modifier)x?.Clone() ?? Loot.Instance.GetNullModifier())
			//		.ToArray();
			Clone(ref clone);
			return clone;
		}

		/// <summary>
		/// Allows the modder to do custom NetReceive
		/// </summary>
		public virtual void NetReceive(Item item, BinaryReader reader)
		{
		}

		/// <summary>
		/// Allows modder to do custom NetSend here
		/// </summary>
		public virtual void NetSend(Item item, BinaryWriter writer)
		{
		}

		/// <summary>
		/// Allows modder to do custom loading here
		/// Use the given TC to pull data you saved using <see cref="Save(Item,TagCompound)"/>
		/// </summary>
		public virtual void Load(Item item, TagCompound tag)
		{
		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(Item, TagCompound)"/>
		/// </summary>
		/// <param name="tag"></param>
		public virtual void Save(Item item, TagCompound tag)
		{
		}
	}
}
