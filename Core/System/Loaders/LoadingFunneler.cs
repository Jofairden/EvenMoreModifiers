using System;
using System.Collections.Generic;
using System.Reflection;
using Loot.Core.ModContent;
using Loot.Core.System.Modifier;
using Loot.Ext.ModSupport;
using Loot.UI.Common;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Loot.Core.System.Loaders
{
	/// <summary>
	/// The LoadingFunneler's purpose is to funnel all loading and unloading to one class
	/// </summary>
	public static class LoadingFunneler
	{
		public static void AddMod(Mod mod)
		{
			MainLoader.RegisterMod(mod);
			MainLoader.AddContent(mod);
		}

		internal static void Load()
		{
			LoadMod();
			if (!Main.dedServ)
			{
				LoadModForClient();
			}
		}

		internal static void Unload()
		{
			UnloadMod();
			Loot.Loaded = false;
		}

		internal static void PostLoad()
		{
			if (!Main.dedServ)
			{
				Loot.ModContentManager.Load();
			}
			Loot.Loaded = true;
		}

		// Load EMM for both Client and Server
		private static void LoadMod()
		{
			ModSupportTunneler.Init();

			ContentLoader.Initialize();
			ContentLoader.Load();
			MainLoader.Initialize();
			MainLoader.Load();

			MainLoader.RegisterMod(Loot.Instance);
			ContentLoader.Modifier.AddContent(typeof(NullModifier), Loot.Instance);
			ContentLoader.ModifierPool.AddContent(typeof(NullModifierPool), Loot.Instance);
			ContentLoader.ModifierRarity.AddContent(typeof(NullModifierRarity), Loot.Instance);
			ContentLoader.ModifierEffect.AddContent(typeof(NullModifierEffect), Loot.Instance);
			MainLoader.AddContent(Loot.Instance);

			if (Main.dedServ)
			{
				ModSupportTunneler.AddServerSupport();
			}
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
		}

		private static void UnloadMod()
		{
			ContentLoader.Unload();
			MainLoader.Unload();
			Loot.ModContentManager?.Unload();
			Loot.ModContentManager = null;
			//UnloadStaticRefs()
		}

		private static void UnloadStaticRefs()
		{
			// TODO causes trouble in unload?
			// Attempt to unload our static variables
			Stack<Type> typesToProcess = new Stack<Type>(Loot.Instance.Code.GetTypes());
			while (typesToProcess.Count > 0)
			{
			    Type type = typesToProcess.Pop();
			    foreach (FieldInfo info in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			    {
			        info.SetValue(null, info.FieldType.IsValueType ? Activator.CreateInstance(info.FieldType) : null);
			    }
			    foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			    {
			        typesToProcess.Push(nestedType);
			    }
			}
		}
	}
}
