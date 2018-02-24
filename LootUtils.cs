using Loot.Modifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Terraria.ModLoader;

namespace Loot
{
	internal static class EMMUtils
	{
		public static ModifierRarity AsNewInstance(this ModifierRarity modifier) => (ModifierRarity)Activator.CreateInstance(modifier.GetType());
		public static ModifierEffect AsNewInstance(this ModifierEffect modifier) => (ModifierEffect)Activator.CreateInstance(modifier.GetType());
		public static Modifier AsNewInstance(this Modifier modifier) => (Modifier)Activator.CreateInstance(modifier.GetType());

		public static T GetModifierRarity<T>(this Mod mod) where T : ModifierRarity => (T)GetModifierRarity(mod, typeof(T).Name);
        public static ModifierRarity GetModifierRarity(this Mod mod, string name)
        {
            List<RarityMap?> v;
            if (EMMLoader.RaritiesMap.TryGetValue(mod.Name, out v))
            {
                var fod = v.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(name));
				return (ModifierRarity)fod?.Rarity.Clone();
            }
            return null;
        }

        public static uint ModifierRarityType<T>(this Mod mod) where T : ModifierRarity => ModifierRarityType(mod, typeof(T).Name);
        public static uint ModifierRarityType(this Mod mod, string name) => GetModifierRarity(mod, name)?.Type ?? 0;

        public static T GetModifierEffect<T>(this Mod mod) where T : ModifierEffect => (T)GetModifierEffect(mod, typeof(T).Name);
        public static ModifierEffect GetModifierEffect(this Mod mod, string name)
        {
            List<EffectMap?> v;
            if (EMMLoader.EffectsMap.TryGetValue(mod.Name, out v))
            {
                var fod = v.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(name));
				return (ModifierEffect)fod?.Effect.Clone();
            }
            return null;
        }

        public static uint ModifierEffectType<T>(this Mod mod) where T : ModifierEffect => ModifierEffectType(mod, typeof(T).Name);
        public static uint ModifierEffectType(this Mod mod, string name) => GetModifierEffect(mod, name)?.Type ?? 0;

		public static T GetModifier<T>(this Mod mod) where T : Modifier => (T)GetModifier(mod, typeof(T).Name);
		public static Modifier GetModifier(this Mod mod, string name)
		{
			List<ModifierMap?> v;
			if (EMMLoader.ModifiersMap.TryGetValue(mod.Name, out v))
			{
				var fod = v.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(name));
				return (Modifier)fod?.Modifier.AsNewInstance().Clone();
			}
			return null;
		}

		public static uint ModifierType<T>(this Mod mod, string name) where T : Modifier => ModifierType(mod, typeof(T).Name);
		public static uint ModifierType(this Mod mod, string name) => GetModifier(mod, name)?.Type ?? 0;

        /// <summary>
        /// Returns the ModifierRarity specified by type, null if not present
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModifierRarity GetRarity(ushort type) => EMMLoader.GetRarity(type);

        /// <summary>
        /// Returns the ModifierEffect specified by type, null if not present
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModifierEffect GetEffect(ushort type) => EMMLoader.GetEffect(type);

        public static byte[] ToByteArray<T>(this T obj)
		{
			if (obj == null)
				return null;
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public static T FromByteArray<T>(this byte[] data)
		{
			if (data == null)
				return default(T);
			BinaryFormatter bf = new BinaryFormatter();
			using (MemoryStream ms = new MemoryStream(data))
			{
				object obj = bf.Deserialize(ms);
				return (T)obj;
			}
		}

		/// <summary>
		/// Attempt to log like a JS object
		/// </summary>
		public static string JSLog(Type type, object instance)
		{
			var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
			var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			string t = "===FIELDS===\n";
			foreach (var info in fields)
			{
				t += $"{info}\n{{{info.GetValue(instance)}}}\n";
			}
			t += "\n===METHODS==\n";
			foreach (var info in methods)
			{
				t += $"{info}\n{{{info.MetadataToken}}}\n";
			}
			return t.Trim();
		}
	}
}
