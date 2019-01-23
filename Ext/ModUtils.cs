using System.Linq;
using Loot.Core.System.Loaders;
using Loot.Core.System.Modifier;
using Terraria.ModLoader;
using EffectMap = System.Collections.Generic.KeyValuePair<string, Loot.Core.System.Modifier.ModifierEffect>;
using ModifierMap = System.Collections.Generic.KeyValuePair<string, Loot.Core.System.Modifier.Modifier>;
using PoolMap = System.Collections.Generic.KeyValuePair<string, Loot.Core.System.Modifier.ModifierPool>;
using RarityMap = System.Collections.Generic.KeyValuePair<string, Loot.Core.System.Modifier.ModifierRarity>;

namespace Loot.Ext
{
	/// <summary>
	/// Defines a set of utility methods for <see cref="Mod"/>
	/// </summary>
	public static class ModUtils
	{
		public static T GetModifierRarity<T>(this Mod mod) where T : ModifierRarity => (T) GetModifierRarity(mod, typeof(T).Name);

		public static ModifierRarity GetModifierRarity(this Mod mod, string name)
		{
			if (ContentLoader.ModifierRarity.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.Value.Name.Equals(name));
				return (ModifierRarity) fod.Value.Clone();
			}

			return null;
		}

		public static uint ModifierRarityType<T>(this Mod mod) where T : ModifierRarity => ModifierRarityType(mod, typeof(T).Name);
		public static uint ModifierRarityType(this Mod mod, string name) => GetModifierRarity(mod, name)?.Type ?? 0;

		public static T GetModifier<T>(this Mod mod) where T : Modifier => (T) GetModifier(mod, typeof(T).Name);

		public static Modifier GetModifier(this Mod mod, string name)
		{
			if (ContentLoader.Modifier.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.Value.Name.Equals(name));
				return (Modifier) fod.Value.Clone();
			}

			return null;
		}

		public static uint ModifierType<T>(this Mod mod) where T : Modifier => ModifierType(mod, typeof(T).Name);
		public static uint ModifierType(this Mod mod, string name) => GetModifier(mod, name)?.Type ?? 0;

		public static T GetModifierPool<T>(this Mod mod) where T : ModifierPool => (T) GetModifierPool(mod, typeof(T).Name);

		public static ModifierPool GetModifierPool(this Mod mod, string name)
		{
			if (ContentLoader.ModifierPool.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.Value.Name.Equals(name));
				return (ModifierPool) fod.Value.Clone();
			}

			return null;
		}

		public static uint ModifierPoolType<T>(this Mod mod, string name) where T : ModifierPool => ModifierPoolType(mod, typeof(T).Name);
		public static uint ModifierPoolType(this Mod mod, string name) => GetModifierPool(mod, name)?.Type ?? 0;

		public static T GetModifierEffect<T>(this Mod mod) where T : ModifierEffect => (T) GetModifierEffect(mod, typeof(T).Name);

		public static ModifierEffect GetModifierEffect(this Mod mod, string name)
		{
			if (ContentLoader.ModifierEffect.Map.TryGetValue(mod.Name, out var v))
			{
				var fod = v.FirstOrDefault(x => x.Value.Name.Equals(name));
				return (ModifierEffect) fod.Value.Clone();
			}

			return null;
		}

		public static uint ModifierEffectType<T>(this Mod mod, string name) where T : ModifierEffect => ModifierEffectType(mod, typeof(T).Name);
		public static uint ModifierEffectType(this Mod mod, string name) => GetModifierEffect(mod, name)?.Type ?? 0;
	}
}
