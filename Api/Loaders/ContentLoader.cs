using Loot.Content;
using Terraria.ModLoader;

namespace Loot.Api.Loaders
{
	/// <summary>
	/// This class holds all Content holders of this mod
	/// You can use this to access content loaded into the mod
	/// Example use: ContentLoader.Modifier.GetContent("mod", "modifier")
	/// </summary>
	public static class ContentLoader
	{
		public static ModifierRarityContent ModifierRarity;
		public static ModifierContent Modifier;
		public static ModifierPoolContent ModifierPool;
		public static ModifierEffectContent ModifierEffect;

		internal static void SkipModChecks(bool val)
		{
			ModifierRarity.SkipModChecks = val;
			Modifier.SkipModChecks = val;
			ModifierPool.SkipModChecks = val;
			ModifierEffect.SkipModChecks = val;
		}

		internal static void Initialize()
		{
			ModifierRarity = new ModifierRarityContent();
			ModifierRarity._Initialize();

			Modifier = new ModifierContent();
			Modifier._Initialize();

			ModifierPool = new ModifierPoolContent();
			ModifierPool._Initialize();

			ModifierEffect = new ModifierEffectContent();
			ModifierEffect._Initialize();
		}

		internal static void Load()
		{
			RegisterMod(Loot.Instance);
			ModifierRarity._Load();
			Modifier._Load();
			ModifierPool._Load();
			ModifierEffect._Load();
		}

		internal static void Unload()
		{
			ModifierRarity._Unload();
			Modifier._Unload();
			ModifierPool._Unload();
			ModifierEffect._Unload();
		}

		internal static void RegisterMod(Mod mod)
		{
			ModifierRarity.AddMod(mod);
			Modifier.AddMod(mod);
			ModifierPool.AddMod(mod);
			ModifierEffect.AddMod(mod);
		}
	}
}
