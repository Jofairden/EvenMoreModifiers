using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Loot.System
{
	public enum ModifierContextMethod
	{
		OnReforge,
		OnCraft,
		OnPickup,
	}

	public struct ModifierContext
	{
		public IDictionary<string, object> CustomData;
		public ModifierContextMethod Method;
		public Player Player;
		public NPC NPC;
		public Item Item;
		public Recipe Recipe;
	}

	/// <summary>
	/// Defines a modifier pool. A modifier pool holds a certain amount of effects in an array
	/// It allows to roll a 'themed' item if it hits an already defined pool
	/// Up to 4 effects are drawn from the pool, and activated
	/// </summary>
	public abstract class ModifierPool : ICloneable, IRollable
	{
		public uint Type { get; internal set; }
		public Mod Mod { get; internal set; }
		public ModifierRarity Rarity { get; internal set; }
		protected internal Modifier[] Effects;
		public Modifier[] Active { get; internal set; }

		public float TotalRarityLevel =>
			Active.Select(effect => effect.RarityLevel).DefaultIfEmpty(0).Sum();

		public IEnumerable<ModifierEffectTooltipLine[]> Description =>
			Active.Select(effect => effect.Description);

		public virtual string Name => GetType().Name;
		public virtual float RollChance => 1f;

		/// <summary>
		/// Returns the ModifierPool specified by type, null if not present
		/// </summary>
		public static ModifierPool GetModifierPool(ushort type)
			=> EMMLoader.GetModifierPool(type);

		public ModifierPool AsNewInstance() 
			=> (ModifierPool)Activator.CreateInstance(GetType());

		internal ModifierRarity UpdateRarity()
		{
			Rarity = EMMLoader.GetPoolRarity(this);
			return Rarity;
		}

		/// <summary>
		/// Roll active modifiers, can roll up to 4 maximum effects
		/// Returns if any modifiers were activated
		/// </summary>
		internal bool RollModifiers(ModifierContext ctx)
		{
			WeightedRandom<Modifier> wr = new WeightedRandom<Modifier>();
			List<Modifier> list = new List<Modifier>();
			foreach (var e in Effects.Where(x => x.CanRoll(ctx)))
				wr.Add(e, e.RollChance);

			for (int i = 0; i < 4; ++i)
			{
				if (wr.elements.Count <= 0 || i > 0 && Main.rand.NextFloat() > ModifierRollChance(i))
					break;

				Modifier e = wr.Get();
				list.Add((Modifier)e.Clone());
				wr.elements.Remove(new Tuple<Modifier, double>(e, e.RollChance));
				wr.needsRefresh = true;
			}

			Active = list.ToArray();
			return Active.Length > 0;
		}

		internal float ModifierRollChance(int len)
			=> 0.5f / (float)Math.Pow(2, len);


		internal bool _CanApply(ModifierContext ctx)
		{
			if (Effects.Length <= 0)
				return false;

			switch (ctx.Method)
			{
				case ModifierContextMethod.OnCraft:
					return CanApplyCraft(ctx);
				case ModifierContextMethod.OnPickup:
					return CanApplyPickup(ctx);
				case ModifierContextMethod.OnReforge:
					return CanApplyReforge(ctx);
				default:
					return true;
			}
		}

		public virtual bool CanRoll(ModifierContext ctx) => Effects.Length > 0 && Effects.Any(x => x.CanRoll(ctx));
		public virtual bool CanApplyCraft(ModifierContext ctx) => Effects.Any(x => x.CanApplyCraft(ctx));
		public virtual bool CanApplyPickup(ModifierContext ctx) => Effects.Any(x => x.CanApplyPickup(ctx));
		public virtual bool CanApplyReforge(ModifierContext ctx) => Effects.Any(x => x.CanApplyReforge(ctx));

		public virtual bool MatchesRarity(ModifierRarity rarity)
			=> TotalRarityLevel >= rarity.RequiredRarityLevel;

		internal void ApplyModifiers(Item item)
		{
			foreach (var effect in Active)
				effect.Apply(item);
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
			clone.Effects =
				Effects?
				.Select(x => x?.Clone())
				.Cast<Modifier>()
				.ToArray();
			clone.Active =
				Active?
				.Select(x => x?.Clone())
				.Cast<Modifier>()
				.ToArray();
			Clone(ref clone);
			return clone;
		}

		/// <summary>
		/// Allows modder to do custom loading here
		/// Use the given TC to pull data you saved using <see cref="Save(TagCompound)"/>
		/// </summary>
		/// <param name="tag"></param>
		public virtual void Load(TagCompound tag)
		{

		}

		/// <summary>
		/// Allows modder to do custom saving here
		/// Use the given TC to put data you want to save, which can be loaded using <see cref="Load(TagCompound)"/>
		/// </summary>
		/// <param name="tag"></param>
		public virtual void Save(TagCompound tag)
		{

		}

		protected internal static ModifierPool _Load(TagCompound tag)
		{
			string modname = tag.GetString("ModName");
			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(modname, out assembly))
			{
				// If we manage to load null here, that means some pool got unloaded
				ModifierPool m;
				try
				{
					m = (ModifierPool) Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));
				}
				catch (Exception)
				{
					return null;
				}

				m.Type = tag.Get<uint>("ModifierType");
				m.Mod = ModLoader.GetMod(modname);
				// preload rarity
				ModifierRarity preloadRarity = ModifierRarity._Load(tag.Get<TagCompound>("Rarity"));
				bool rarityUnloaded = preloadRarity == null;
				if (!rarityUnloaded)
					m.Rarity = preloadRarity;
				int effects = tag.GetAsInt("Effects");
				if (effects > 0)
				{
					var list = new List<Modifier>();
					for (int i = 0; i < effects; ++i)
					{
						// preload to take unloaded modifiers into account
						var loaded = Modifier._Load(tag.Get<TagCompound>($"Effect{i}"));
						if (loaded != null)
							list.Add(loaded);
					}
					m.Effects = list.ToArray();
				}
				int activeEffects = tag.GetAsInt("ActiveEffects");
				if (activeEffects > 0)
				{
					var list = new List<Modifier>();
					for (int i = 0; i < activeEffects; ++i)
					{
						// preload to take unloaded modifiers into account
						var loaded = Modifier._Load(tag.Get<TagCompound>($"ActiveEffect{i}"));
						if (loaded != null)
							list.Add(loaded);
					}
					m.Active = list.ToArray();
				}
				m.Load(tag);

				// If our rarity was unloaded, attempt rolling a new one that is applicable
				if (rarityUnloaded)
					m.Rarity = EMMLoader.GetPoolRarity(m);
				return m;
			}
			throw new Exception($"Modifier load error for {modname}");
		}

		protected internal static TagCompound Save(ModifierPool modifierPool)
		{
			var tag = new TagCompound
			{
				{"Type", modifierPool.GetType().FullName },
				{"ModifierType", modifierPool.Type },
				{"ModName", modifierPool.Mod.Name },
				{"Rarity", ModifierRarity.Save(modifierPool.Rarity) },
			};
			tag.Add("Effects", modifierPool.Effects.Length);
			if (modifierPool.Effects.Length > 0)
			{
				for (int i = 0; i < modifierPool.Effects.Length; ++i)
				{
					tag.Add($"Effect{i}", Modifier.Save(modifierPool.Effects[i]));
				}
			}
			tag.Add("ActiveEffects", modifierPool.Active.Length);
			for (int i = 0; i < modifierPool.Active.Length; ++i)
			{
				tag.Add($"ActiveEffect{i}", Modifier.Save(modifierPool.Active[i]));
			}
			modifierPool.Save(tag);
			return tag;
		}


		public override string ToString()
			=> EMMUtils.JSLog(typeof(ModifierPool), this);
	}
}
