using Loot.Core.System.Modifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;

namespace Loot.Core.System.Loaders
{
	/// <summary>
	/// The MainLoader is responsible for loading mods into EMM
	/// Mods can be registered and triggered to have their content
	/// added via this class
	/// </summary>
	public static class MainLoader
	{
		internal static IDictionary<string, Assembly> Mods;
		public static int ModCount => Mods.Count;

		internal static void Initialize()
		{
			Mods = new Dictionary<string, Assembly>();
		}

		internal static void Load()
		{
		}

		internal static void Unload()
		{
			Mods.Clear();
		}

		internal static void CheckModLoading(Mod mod, string source)
		{
			if (mod == null)
			{
				throw new NullReferenceException($"Mod is null in {source}");
			}

			if (mod.GetType().GetField("loading", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(mod) is bool b && !b)
			{
				throw new Exception($"{source} can only be called from Mod.Load or Mod.Autoload");
			}
		}

		internal static void CheckModRegistered(Mod mod)
		{
			if (!Mods.ContainsKey(mod.Name))
			{
				throw new Exception($"Mod {mod.Name} is not registered, please register before adding");
			}
		}

		/// <summary>
		/// Registers specified mod, enabling autoloading for that mod
		/// </summary>
		public static void RegisterMod(Mod mod)
		{
			CheckModLoading(mod, "RegisterMod");
			if (Mods.ContainsKey(mod.Name))
			{
				throw new Exception($"Mod {mod.Name} is already registered");
			}

			Assembly code = mod.Code;

			Mods.Add(new KeyValuePair<string, Assembly>(mod.Name, code));
			ContentLoader.RegisterMod(mod);
		}

		public static void AddContent(Mod mod)
		{
			CheckModLoading(mod, "SetupContent");
			CheckModRegistered(mod);

			var ordered = Mods.FirstOrDefault(x => x.Key.Equals(mod.Name))
				.Value
				.GetTypes()
				.OrderBy(x => x.FullName, StringComparer.InvariantCulture)
				.Where(t => t.IsClass && !t.IsAbstract)
				.ToList();

			var rarities = ordered.Where(x => x.IsSubclassOf(typeof(ModifierRarity)));
			var modifiers = ordered.Where(x => x.IsSubclassOf(typeof(Modifier.Modifier)));
			var pools = ordered.Where(x => x.IsSubclassOf(typeof(ModifierPool)));
			var effects = ordered.Where(x => x.IsSubclassOf(typeof(ModifierEffect)));

			// Kinda meh, but no need to recheck here..
			ContentLoader.ModifierRarity.SkipModChecks = true;
			ContentLoader.Modifier.SkipModChecks = true;
			ContentLoader.ModifierPool.SkipModChecks = true;
			ContentLoader.ModifierEffect.SkipModChecks = true;

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

			ContentLoader.ModifierRarity.SkipModChecks = false;
			ContentLoader.Modifier.SkipModChecks = false;
			ContentLoader.ModifierPool.SkipModChecks = false;
			ContentLoader.ModifierEffect.SkipModChecks = false;
		}
	}
}
