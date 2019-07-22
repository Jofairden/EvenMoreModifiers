using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loot.Api.Core;
using Terraria.ModLoader;

namespace Loot.Api.Attributes
{
	/// <inheritdoc cref="Attribute"/>
	/// <summary>
	/// Will populate a ModifierPool based on a namespace
	/// All Modifiers in that namespace will become part of the pool
	/// You can specify multiple namespaces
	/// Note: modifiers you return in GetModifiers() are also still added!
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PopulatePoolFromAttribute : Attribute
	{
		public string[] Namespaces { get; set; }

		public PopulatePoolFromAttribute(params string[] namespaces)
		{
			Assembly asm = Assembly.GetExecutingAssembly();

			// TODO custom assembly not tested!
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
					useAssembly = mod.Code;
				}

				if (!useAssembly.GetTypes().Any(x => x.Namespace != null && x.Namespace.StartsWith(ns)))
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

		private IEnumerable<Type> GetModifierClasses(string ns)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
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
				useAssembly = mod.Code;
			}

			return useAssembly
				.GetTypes()
				.Where(type =>
					type.IsClass
					&& type.IsSubclassOf(typeof(Modifier))
					&& type.Namespace != null && type.Namespace.StartsWith(ns));
		}
	}
}
