using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loot.Core.System.Content;
using Terraria.ModLoader;

namespace Loot.Core.System.Loaders
{
	public static class ContentLoader
	{
		public static ModifierRarityContent ModifierRarity;
		public static ModifierContent Modifier;
		public static ModifierPoolContent ModifierPool;
		public static ModifierEffectContent ModifierEffect;

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
