﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Loot.System;
using Terraria.ModLoader;

using RarityMap = System.Collections.Generic.KeyValuePair<string, Loot.System.ModifierRarity>;
using ModifierMap = System.Collections.Generic.KeyValuePair<string, Loot.System.Modifier>;
using PoolMap = System.Collections.Generic.KeyValuePair<string, Loot.System.ModifierPool>;

namespace Loot
{
	internal static class EMMUtils
	{
		public static T GetModifierRarity<T>(this Mod mod) where T : ModifierRarity => (T)GetModifierRarity(mod, typeof(T).Name);
		public static ModifierRarity GetModifierRarity(this Mod mod, string name)
		{
			List<RarityMap> v;
			if (EMMLoader.RaritiesMap.TryGetValue(mod.Name, out v))
			{
				var fod = v.FirstOrDefault(x => x.Value.Name.Equals(name));
				return (ModifierRarity)fod.Value.Clone();
			}
			return null;
		}

		public static uint ModifierRarityType<T>(this Mod mod) where T : ModifierRarity => ModifierRarityType(mod, typeof(T).Name);
		public static uint ModifierRarityType(this Mod mod, string name) => GetModifierRarity(mod, name)?.Type ?? 0;

		public static T GetModifier<T>(this Mod mod) where T : Modifier => (T)GetModifier(mod, typeof(T).Name);
		public static Modifier GetModifier(this Mod mod, string name)
		{
			List<ModifierMap> v;
			if (EMMLoader.ModifiersMap.TryGetValue(mod.Name, out v))
			{
				var fod = v.FirstOrDefault(x => x.Value.Name.Equals(name));
				return (Modifier)fod.Value.Clone();
			}
			return null;
		}

		public static uint ModifierType<T>(this Mod mod) where T : Modifier => ModifierType(mod, typeof(T).Name);
		public static uint ModifierType(this Mod mod, string name) => GetModifier(mod, name)?.Type ?? 0;

		public static T GetModifierPool<T>(this Mod mod) where T : ModifierPool => (T)GetModifierPool(mod, typeof(T).Name);
		public static ModifierPool GetModifierPool(this Mod mod, string name)
		{
			List<PoolMap> v;
			if (EMMLoader.PoolsMap.TryGetValue(mod.Name, out v))
			{
				var fod = v.FirstOrDefault(x => x.Value.Name.Equals(name));
				return (ModifierPool)fod.Value.Clone();
			}
			return null;
		}

		public static uint ModifierPoolType<T>(this Mod mod, string name) where T : ModifierPool => ModifierPoolType(mod, typeof(T).Name);
		public static uint ModifierPoolType(this Mod mod, string name) => GetModifierPool(mod, name)?.Type ?? 0;

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
