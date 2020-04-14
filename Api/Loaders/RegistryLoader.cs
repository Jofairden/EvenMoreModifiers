using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Loot.Api.Attributes;
using Loot.Api.Core;
using Terraria.ModLoader;

namespace Loot.Api.Loaders
{
	/// <summary>
	/// This class is responsible for loading mods into Even More Modifiers
	/// Mods can be registered and triggered to have their content added via this class
	/// </summary>
	public static class RegistryLoader
	{
		internal static IDictionary<string, Mod> Mods;
		public static int ModCount => Mods.Count;

		internal static void Initialize()
		{
			Mods = new Dictionary<string, Mod>();
		}

		internal static void Load()
		{
			Mods.Add(Loot.Instance.Name, Loot.Instance);
		}

		internal static void Unload()
		{
			Mods?.Clear();
			Mods = null;
		}

		internal static void CheckModLoading(Mod mod, string source, bool invert = false)
		{
			if (mod == null)
			{
				throw new NullReferenceException($"Mod is null in {source}");
			}

			if (mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) is bool b)
			{
				if (!invert && !b)
				{
					throw new Exception($"{source} can only be called from Mod.Load or Mod.Autoload");
				}

				if (invert && b)
				{
					throw new Exception($"{source} can only be called if mod {mod.Name} is already loaded");
				}
			}
		}

		internal static bool CheckModRegistered(Mod mod, string source, bool @throw = true)
		{
			bool modIsPresent = Mods.ContainsKey(mod.Name);
			if (modIsPresent && @throw)
			{
				throw new Exception($"Mod {mod.Name} is already registered in {source}");
			}

			return modIsPresent;
		}

		/// <summary>
		/// Registers specified mod, enabling autoloading for that mod
		/// </summary>
		public static void RegisterMod(Mod mod)
		{
			CheckModLoading(mod, "RegisterMod");
			CheckModRegistered(mod, "RegisterMod");

			Mods.Add(new KeyValuePair<string, Mod>(mod.Name, mod));
			ContentLoader.RegisterMod(mod);
		}

		internal static void ProcessMods()
		{
			foreach (var kvp in Mods)
			{
				AddContent(kvp.Value);
			}
		}

		/// <summary>
		/// Adds content for the specified mod.
		/// This will add all the <see cref="Modifier"/>, <see cref="ModifierRarity"/>, <see cref="ModifierPool"/> and <see cref="ModifierEffect"/> classes
		/// </summary>
		/// <param name="mod"></param>
		internal static void AddContent(Mod mod)
		{
			CheckModLoading(mod, "SetupContent", invert: true);

			var ordered = Mods.FirstOrDefault(x => x.Key.Equals(mod.Name))
				.Value
				.Code
				.GetTypes()
				.OrderBy(x => x.FullName, StringComparer.InvariantCulture)
				.Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<DoNotLoadAttribute>() == null)
				.ToList();

			var rarities = ordered.Where(x => x.IsSubclassOf(typeof(ModifierRarity)));
			var modifiers = ordered.Where(x => x.IsSubclassOf(typeof(Modifier)));
			var pools = ordered.Where(x => x.IsSubclassOf(typeof(ModifierPool)));
			var effects = ordered.Where(x => x.IsSubclassOf(typeof(ModifierEffect)));

			// Kinda meh, but no need to recheck here..
			ContentLoader.SkipModChecks(true);

			// important: load things in order. (modifiers relies on all.. etc.)
			foreach (Type type in rarities)
			{
				ContentLoader.ModifierRarity.AddContent(type, mod);
			}

			foreach (Type type in modifiers)
			{
				ContentLoader.Modifier.AddContent(type, mod);
			}

			foreach (Type type in pools)
			{
				ContentLoader.ModifierPool.AddContent(type, mod);
			}

			foreach (Type type in effects)
			{
				ContentLoader.ModifierEffect.AddContent(type, mod);
			}

			ContentLoader.SkipModChecks(false);
		}
	}
}
