using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace Loot.Ext.ModSupport
{
	internal static class ModSupport
	{
		private static List<ModSupporter> _modSupporters;

		public static T GetSupport<T>() where T : ModSupporter
		{
			return (T)_modSupporters?.FirstOrDefault(s => s.GetType() == typeof(T) || s.GetType().IsAssignableFrom(typeof(T)));
		}

		public static void Init()
		{
			foreach (var modSupporter in GetSupporters())
			{
				Mod supportingMod = modSupporter.GetSupportingMod();
				modSupporter.ModIsLoaded = modSupporter.CheckValidity(supportingMod);
			}
		}

		public static void AddServerSupport()
		{
			foreach (var modSupporter in GetSupporters())
			{
				Mod supportingMod = modSupporter.GetSupportingMod();
				if (modSupporter.ModIsLoaded)
				{
					modSupporter.AddServerSupport(supportingMod);
				}
			}
		}

		public static void AddClientSupport()
		{
			foreach (var modSupporter in GetSupporters())
			{
				Mod supportingMod = modSupporter.GetSupportingMod();
				if (modSupporter.ModIsLoaded)
				{
					modSupporter.AddClientSupport(supportingMod);
				}
			}
		}

		private static IEnumerable<ModSupporter> GetSupporters()
		{
			if (_modSupporters == null)
			{
				// Not loaded yet, load them dynamically
				string root = typeof(ModSupport).Namespace;
				_modSupporters = Assembly.GetExecutingAssembly().GetTypes()
					.Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null && t.Namespace.StartsWith(root)
								&& t.IsSubclassOf(typeof(ModSupporter)))
					.Select(t =>
					{
						return (ModSupporter) Activator.CreateInstance(t);
					})
					.ToList();
			}

			return _modSupporters;
		}
	}
}
