using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Loot.Modifiers
{
	public enum ModifierContextMethod
	{
		OnReforge,
		OnCraft,
		OnPickup,
		UpdateItem,
		UpdateNPC
	}

	public struct ModifierContext
	{
		public ModifierContextMethod Method;
		public Player Player;
		public NPC NPC;
		public Item Item;
		public IDictionary<string, object> CustomData;
	}

	/// <summary>
	/// Defines a modifier.
	/// </summary>
	[Serializable]
	public abstract class Modifier : ICloneable, TagSerializable
	{
		public uint Type { get; internal set; }
		public Mod Mod { get; internal set; }
		public ModifierRarity Rarity { get; internal set; }
		protected internal ModifierEffect[] Effects;
		public ModifierEffect[] ActiveEffects { get; internal set; }

		public float TotalRarityLevel =>
			ActiveEffects.Select(effect => effect.RarityLevel).DefaultIfEmpty(0).Sum();

		public IEnumerable<ModifierEffectTooltipLine[]> Description =>
			ActiveEffects.Select(effect => effect.Description);

		public virtual string Name => GetType().Name;
		public virtual float RollChance => 1f;

		internal ModifierRarity UpdateRarity()
		{
			Rarity = EMMLoader.GetModifierRarity(this);
			return Rarity;
		}

		internal ModifierEffect[] RollEffects(ModifierContext ctx)
		{
			WeightedRandom<ModifierEffect> wr = new WeightedRandom<ModifierEffect>();
			List<ModifierEffect> list = new List<ModifierEffect>();
			foreach (var e in Effects.Where(x => x.CanRoll(ctx)))
				wr.Add(e, e.RollChance);

			if (wr.elements.Count <= 0)
				return ActiveEffects;

			for (int i = 0; i < 4; ++i)
			{
				if (wr.elements.Count <= 0 || Main.rand.NextFloat() > (EffectRollChance(i)))
					break;

				ModifierEffect e = wr.Get();
				list.Add((ModifierEffect)e.Clone());
				wr.elements.Remove(new Tuple<ModifierEffect, double>(e, e.RollChance));
				wr.needsRefresh = true;
			}

			ActiveEffects = list.ToArray();
			return ActiveEffects;
		}

		internal float EffectRollChance(int len)
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
				case ModifierContextMethod.UpdateItem:
					return CanUpdateItem(ctx);
				case ModifierContextMethod.UpdateNPC:
					return CanUpdateItem(ctx);
				default:
					return true;
			}
		}

		public virtual bool CanRoll(ModifierContext ctx) => Effects.Length > 0 && Effects.Any(x => x.CanRoll(ctx));
		public virtual bool CanApplyCraft(ModifierContext ctx) => true;
		public virtual bool CanApplyPickup(ModifierContext ctx) => true;
		public virtual bool CanApplyReforge(ModifierContext ctx) => true;
		public virtual bool CanApplyItem(ModifierContext ctx) => true;
		public virtual bool CanUpdateItem(ModifierContext ctx) => true;
		public virtual bool CanUpdateNPC(ModifierContext ctx) => true;

		public virtual bool MatchesRarity(ModifierRarity rarity)
			=> TotalRarityLevel >= rarity.RequiredRarityLevel;

		internal void ApplyItem(ModifierContext ctx)
		{
			foreach (var effect in ActiveEffects)
				effect.ApplyItem(ctx);
		}

		internal void UpdateItem(ModifierContext ctx, bool equipped = false)
		{
			foreach (var effect in ActiveEffects)
				effect.UpdateItem(ctx, equipped);
		}

		public override string ToString()
			=> EMMUtils.JSLog(typeof(Modifier), this);

		public virtual void Clone(ref Modifier clone)
		{

		}

		public object Clone()
		{
			Modifier clone = (Modifier)MemberwiseClone();
			clone.Type = Type;
			clone.Mod = Mod;
			clone.Rarity = (ModifierRarity)Rarity?.Clone();
			clone.Effects =
				Effects?
				.Select(x => x?.Clone())
				.Cast<ModifierEffect>()
				.ToArray();
			clone.ActiveEffects =
				ActiveEffects?
				.Select(x => x?.Clone())
				.Cast<ModifierEffect>()
				.ToArray();
			Clone(ref clone);
			return clone;
		}

		//protected static TagCompound Save(Modifier modifier)
		//{
		//	var tc = new TagCompound
		//	{
		//		{"Assembly", modifier.GetType().AssemblyQualifiedName },
		//		{"TypeName", modifier.GetType().Name },
		//		{"ModifierType", modifier.Type },
		//		{"ModName", modifier.Mod.Name },
		//		{"Rarity", ModifierRarity.Save(modifier.Rarity) },
		//	};
		//	tc.Add("Effects", modifier.Effects.Length);
		//	if (modifier.Effects.Length > 0)
		//	{
		//		for (int i = 0; i < modifier.Effects.Length; ++i)
		//		{
		//			tc.Add($"Effect{i}", ModifierEffect.Save(modifier.Effects[i]));
		//		}
		//	}
		//	tc.Add("ActiveEffects", modifier.ActiveEffects.Length);
		//	for (int i = 0; i < modifier.ActiveEffects.Length; ++i)
		//	{
		//		tc.Add($"ActiveEffect{i}", ModifierEffect.Save(modifier.ActiveEffects[i]));
		//	}
		//	return tc;
		//}

		protected internal static Modifier Load(TagCompound tag)
		{
			string modname = tag.GetString("ModName");
			Assembly assembly;
			if (EMMLoader.Mods.TryGetValue(modname, out assembly))
			{
				Modifier m = (Modifier)Activator.CreateInstance(assembly.GetType(tag.GetString("Type")));
				m.Type = tag.Get<uint>("ModifierType");
				m.Mod = ModLoader.GetMod(modname);
				m.Rarity = tag.Get<ModifierRarity>("Rarity");
				int effects = tag.GetAsInt("Effects");
				if (effects > 0)
				{
					var list = new List<ModifierEffect>();
					for (int i = 0; i < effects; ++i)
					{
						list.Add(tag.Get<ModifierEffect>($"Effect{i}"));
					}
					m.Effects = list.ToArray();
				}
				int activeeffects = tag.GetAsInt("ActiveEffects");
				if (activeeffects > 0)
				{
					var list = new List<ModifierEffect>();
					for (int i = 0; i < activeeffects; ++i)
					{
						list.Add(tag.Get<ModifierEffect>($"ActiveEffect{i}"));
					}
					m.ActiveEffects = list.ToArray();
				}
				return m;
			}
			throw new Exception($"Modifier load error for {modname}");
		}

		public static Func<TagCompound, Modifier> DESERIALIZER = tag => Load(tag);

		public TagCompound SerializeData()
		{
			var modifier = this;
			var tc = new TagCompound
			{
				{"Type", modifier.GetType().FullName },
				{"ModifierType", modifier.Type },
				{"ModName", modifier.Mod.Name },
				{"Rarity", modifier.Rarity },
			};
			tc.Add("Effects", modifier.Effects.Length);
			if (modifier.Effects.Length > 0)
			{
				for (int i = 0; i < modifier.Effects.Length; ++i)
				{
					tc.Add($"Effect{i}", modifier.Effects[i]);
				}
			}
			tc.Add("ActiveEffects", modifier.ActiveEffects.Length);
			for (int i = 0; i < modifier.ActiveEffects.Length; ++i)
			{
				tc.Add($"ActiveEffect{i}", modifier.ActiveEffects[i]);
			}
			return tc;
		}
	}
}
