using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Loot
{
	internal static class LootUtils
	{
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
