using System;
using System.Linq;
using Loot.Api.Core;
using Loot.Api.Loaders;
using Loot.Pools;
using Terraria.ModLoader;

namespace Loot.Api.Ext
{
	/// <summary>
	/// Defines a set of utility methods for Mod class
	/// </summary>
	public static class ModUtils
	{
		public static NullModifier GetNullModifier(this Mod mod) => Loot.Instance.GetModifier<NullModifier>();
		public static NullModifierPool GetNullModifierPool(this Mod mod) => Loot.Instance.GetModifierPool<NullModifierPool>();
		public static NullModifierRarity GetNullModifierRarity(this Mod mod) => Loot.Instance.GetModifierRarity<NullModifierRarity>();
		public static NullModifierEffect GetNullModifierEffect(this Mod mod) => Loot.Instance.GetModifierEffect<NullModifierEffect>();

		public static AllModifiersPool GetAllModifiersPool(this Mod mod) => Loot.Instance.GetModifierPool<AllModifiersPool>();

		public static Modifier GetModifier(Type type) => ContentLoader.Modifier.GetContent(type);
		public static ModifierPool GetModifierPool(Type type) => ContentLoader.ModifierPool.GetContent(type);
		public static ModifierRarity GetModifierRarity(Type type) => ContentLoader.ModifierRarity.GetContent(type);
		public static ModifierEffect GetModifierEffect(Type type) => ContentLoader.ModifierEffect.GetContent(type);

		public static T GetModifierRarity<T>(this Mod mod) where T : ModifierRarity => (T)GetModifierRarity(mod, typeof(T).Name);
		public static ModifierRarity GetModifierRarity(this Mod mod, Type type) => GetModifierRarity(mod, type.Name);

		public static ModifierRarity GetModifierRarity(this Mod mod, string name)
		{
			if (ContentLoader.ModifierRarity.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.content.Name.Equals(name));
				return (ModifierRarity)fod.content.Clone();
			}

			return null;
		}

		public static uint GetModifierRarityType<T>(this Mod mod) where T : ModifierRarity => GetModifierRarityType(mod, typeof(T).Name);
		public static uint GetModifierRarityType(this Mod mod, Type type) => GetModifierRarityType(mod, type.Name);
		public static uint GetModifierRarityType(this Mod mod, string name) => GetModifierRarity(mod, name)?.Type ?? 0;

		public static T GetModifier<T>(this Mod mod) where T : Modifier => (T)GetModifier(mod, typeof(T).Name);
		public static Modifier GetModifier(this Mod mod, Type type) => GetModifier(mod, type.Name);

		public static Modifier GetModifier(this Mod mod, string name)
		{
			if (ContentLoader.Modifier.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.content.Name.Equals(name));
				return (Modifier)fod.content.Clone();
			}

			return null;
		}

		public static uint GetModifierType<T>(this Mod mod) where T : Modifier => GetModifierType(mod, typeof(T).Name);
		public static uint GetModifierType(this Mod mod, Type type) => GetModifierType(mod, type.Name);
		public static uint GetModifierType(this Mod mod, string name) => GetModifier(mod, name)?.Type ?? 0;

		public static T GetModifierPool<T>(this Mod mod) where T : ModifierPool => (T)GetModifierPool(mod, typeof(T).Name);
		public static ModifierPool GetModifierPool(this Mod mod, Type type) => GetModifierPool(mod, type.Name);

		public static ModifierPool GetModifierPool(this Mod mod, string name)
		{
			if (ContentLoader.ModifierPool.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.content.Name.Equals(name));
				return (ModifierPool)fod.content.Clone();
			}

			return null;
		}

		public static uint GetModifierPoolType<T>(this Mod mod, string name) where T : ModifierPool => GetModifierPoolType(mod, typeof(T).Name);
		public static uint GetModifierPoolType(this Mod mod, Type type) => GetModifierPoolType(mod, type.Name);
		public static uint GetModifierPoolType(this Mod mod, string name) => GetModifierPool(mod, name)?.Type ?? 0;

		public static T GetModifierEffect<T>(this Mod mod) where T : ModifierEffect => (T)GetModifierEffect(mod, typeof(T).Name);
		public static ModifierEffect GetModifierEffect(this Mod mod, Type type) => GetModifierEffect(mod, type.Name);

		public static ModifierEffect GetModifierEffect(this Mod mod, string name)
		{
			if (ContentLoader.ModifierEffect.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.content.Name.Equals(name));
				return (ModifierEffect)fod.content.Clone();
			}

			return null;
		}

		public static uint GetModifierEffectType<T>(this Mod mod, string name) where T : ModifierEffect => GetModifierEffectType(mod, typeof(T).Name);
		public static uint GetModifierEffectType(this Mod mod, Type type) => GetModifierEffectType(mod, type.Name);
		public static uint GetModifierEffectType(this Mod mod, string name) => GetModifierEffect(mod, name)?.Type ?? 0;
	}
}
