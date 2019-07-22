using Loot.Api.ModContent;
using Loot.Hacks;
using Loot.Modifiers;
using Loot.ModSupport;
using Loot.Pools;
using Loot.Rarities;
using Loot.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.Api.Loaders
{
	/// <summary>
	/// The LoadingFunneler's purpose is to funnel all loading and unloading to one class
	/// </summary>
	public static class LoadingFunneler
	{
		internal static void Load()
		{
			LoadMod();
			if (!Main.dedServ)
			{
				LoadModForClient();
			}
			else
			{
				ModSupportTunneler.AddServerSupport();
			}
		}

		internal static void Unload()
		{
			UnloadMod();
			Loot.IsLoaded = false;
		}

		internal static void PostLoad()
		{
			if (!Main.dedServ)
			{
				Loot.ModContentManager.Load();
			}
			Loot.IsLoaded = true;
		}

		// Load EMM for both Client and Server
		private static void LoadMod()
		{
			ModSupportTunneler.Init();

			RegistryLoader.Initialize();
			RegistryLoader.Load();
			ContentLoader.Initialize();
			ContentLoader.Load();

			RegistryLoader.AddContent(Loot.Instance);
		}

		// Load EMM for Client only (this doesn't need to be loaded for server)
		private static void LoadModForClient()
		{
			Loot.ModContentManager = new ModContentManager();
			Loot.ModContentManager.Initialize(Loot.Instance);

			Loot.Instance.GuiInterface = new UserInterface();
			Loot.Instance.GuiState = new GuiTabWindow();
			Loot.Instance.GuiState.Activate();

			AssetLoader.RegisterAssets(Loot.Instance, "GraphicsAssets");
			ModSupportTunneler.AddClientSupport();

			// TODO IL patch management
			new UseItemEffectsHack().Hack();
		}

		private static void UnloadMod()
		{
			ContentLoader.Unload();
			RegistryLoader.Unload();
			Loot.ModContentManager?.Unload();
			Loot.ModContentManager = null;
			//UnloadStaticRefs()
		}

		// TODO unused
		//private static void UnloadStaticRefs()
		//{
		//	// TODO causes trouble in unload?
		//	// Attempt to unload our static variables
		//	Stack<Type> typesToProcess = new Stack<Type>(Loot.Instance.Code.GetTypes());
		//	while (typesToProcess.Count > 0)
		//	{
		//	    Type type = typesToProcess.Pop();
		//	    foreach (FieldInfo info in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
		//	    {
		//	        info.SetValue(null, info.FieldType.IsValueType ? Activator.CreateInstance(info.FieldType) : null);
		//	    }
		//	    foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
		//	    {
		//	        typesToProcess.Push(nestedType);
		//	    }
		//	}
		//}
	}
}
