using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loot.Attributes;
using Loot.ILEditing;

namespace Loot.Ext
{
	internal static class ReflectUtils
	{
		public static IEnumerable<Type> GetILEdits()
			=> GetLootNonAbstractClasses(t => t.IsSubclassOf(typeof(ILEdit)));

		public static IEnumerable<MemberInfo> GetStaticAssetTypes()
		{
			return GetStaticAssetTypesTailRec(Loot.Instance.Code.GetTypes(), typeof(StaticAssetAttribute));
		}

		// Tailrecursive call to iterate over all nested types
		private static IEnumerable<MemberInfo> GetStaticAssetTypesTailRec(Type[] arr, Type attr, IEnumerable<MemberInfo> list = null, IEnumerable<Type> toCheck = null)
		{
			bool HasAttr(MemberInfo i) => Attribute.IsDefined(i, attr);

			IEnumerable<MemberInfo> GetPropsWithAttr(IEnumerable<Type> col)
			{
				//const BindingFlags flags = BindingFlags.Static;
				return col.SelectMany(t => t.GetFields()).Where(f => f.IsStatic).Where(HasAttr).Cast<MemberInfo>()
					.Concat(col.SelectMany(t => t.GetProperties()).Where(p => p.GetAccessors().Any(a => a.IsStatic)).Where(HasAttr));
			}

			var types = list ?? GetPropsWithAttr(arr);
			var nested = (toCheck ?? arr).SelectMany(t => t.GetNestedTypes()).ToArray();
			if (nested.Length > 0)
			{
				types = types.Union(GetPropsWithAttr(nested));
				types = GetStaticAssetTypesTailRec(arr, attr, types, nested);
			}

			return types;
		}

		private static IEnumerable<Type> GetLootNonAbstractClasses(Func<Type, bool> fun = null)
			=> Loot.Instance.Code.GetTypes().Where(t => t.IsClass && !t.IsAbstract && (fun?.Invoke(t) ?? true));
	}
}
