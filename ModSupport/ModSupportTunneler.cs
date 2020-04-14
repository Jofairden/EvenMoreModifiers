using System;
using System.Collections.Generic;
using System.Linq;
using Loot.Ext;
using Terraria.ModLoader;

namespace Loot.ModSupport
{
	/// <summary>
	/// The ModSupportTunneler is responsible for managing ModSupport classes and adding their implemented support to the mod
	/// (This is used for cross-mod compatibility)
	/// </summary>
	internal static class ModSupportTunneler
	{
		private static List<ModSupport> _modSupporters;

		public static T GetModSupport<T>() where T : ModSupport
		{
			return (T)_modSupporters?.FirstOrDefault(s => s.GetType() == typeof(T) || s.GetType().IsAssignableFrom(typeof(T)));
		}

		public static void Init()
		{
			foreach (var modSupporter in GetSupporters())
			{
				Mod supportingMod = modSupporter.GetSupportingMod();
				modSupporter.ModIsLoaded = supportingMod != null && modSupporter.CheckValidity(supportingMod);
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

		private static IEnumerable<ModSupport> GetSupporters()
		{
			return _modSupporters ??
				   (_modSupporters =
					   ReflectUtils.GetLootNonAbstractClasses(t => t.IsSubclassOf(typeof(ModSupport)))
					   .Select(t => (ModSupport)Activator.CreateInstance(t))
					   .ToList());
		}
	}
}
