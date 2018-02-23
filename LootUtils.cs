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
	internal static class LootUtils
	{
        public static T GetModifierRarity<T>(this Mod mod) where T : ModifierRarity => (T)GetModifierRarity(mod, typeof(T).Name);
        public static ModifierRarity GetModifierRarity(this Mod mod, string name)
        {
            List<RarityMap?> v;
            if (LootLoader.RaritiesMap.TryGetValue(mod.Name, out v))
            {
                var fod = v.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(name));
                return fod?.Rarity;
            }
            return null;
        }

        public static uint ModifierRarityType<T>(this Mod mod) where T : ModifierRarity => ModifierRarityType(mod, typeof(T).Name);
        public static uint ModifierRarityType(this Mod mod, string name) => GetModifierRarity(mod, name)?.Type ?? 0;

        public static T GetModifierEffect<T>(this Mod mod) where T : ModifierEffect => (T)GetModifierEffect(mod, typeof(T).Name);
        public static ModifierEffect GetModifierEffect(this Mod mod, string name)
        {
            List<EffectMap?> v;
            if (LootLoader.EffectsMap.TryGetValue(mod.Name, out v))
            {
                var fod = v.FirstOrDefault(x => x.HasValue && x.Value.Name.Equals(name));
                return fod?.Effect;
            }
            return null;
        }

        public static uint ModifierEffectType<T>(this Mod mod) where T : ModifierEffect => ModifierEffectType(mod, typeof(T).Name);
        public static uint ModifierEffectType(this Mod mod, string name) => GetModifierEffect(mod, name)?.Type ?? 0;

        /// <summary>
        /// Returns the ModifierRarity specified by type, null if not present
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModifierRarity GetRarity(ushort type) => LootLoader.GetRarity(type);

        /// <summary>
        /// Returns the ModifierEffect specified by type, null if not present
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModifierEffect GetEffect(ushort type) => LootLoader.GetEffect(type);

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
