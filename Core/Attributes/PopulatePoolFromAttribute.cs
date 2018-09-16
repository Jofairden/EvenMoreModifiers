using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace Loot.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PopulatePoolFromAttribute : Attribute
	{
		public string[] Namespaces { get; set; }

		public PopulatePoolFromAttribute(params string[] namespaces)
		{
			Assembly asm = Assembly.GetExecutingAssembly();

			// @todo custom assembly not tested!
			foreach (string ns in namespaces)
			{
				Assembly useAssembly = asm;
				string root = ns;
				int index = ns.IndexOf('.');
				if (index > 0)
				{
					root = ns.Substring(0, index);
				}

				var mod = ModLoader.GetMod(root);
				if (mod != null)
				{
					asm = mod.Code;
				}

				// ReSharper disable once SimplifyLinqExpression
				if (!useAssembly.GetTypes().Any(x => x.Namespace == ns))
				{
					throw new ArgumentException($"Namespace not found", nameof(ns));
				}
			}
			Namespaces = namespaces;
		}

		public Dictionary<string, List<Type>> GetClasses()
		{
			var dict = new Dictionary<string, List<Type>>();
			foreach (string s in Namespaces)
			{
				dict.Add(s, GetModifierClasses(s).ToList());
			}
			return dict;
		}

		private IEnumerable<Type> GetModifierClasses(string nameSpace)
		{
			Assembly asm = Assembly.GetExecutingAssembly();

			return asm.GetTypes()
				.Where(type =>
					type.IsClass
					&& type.IsSubclassOf(typeof(Modifier))
					&& type.Namespace == nameSpace);
		}
	}
}
