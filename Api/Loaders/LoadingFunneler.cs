using System;
using System.Diagnostics;
using System.Reflection;
using Loot.Api.ModContent;
using Loot.Attributes;
using Loot.Ext;
using Loot.ILEditing;
using Loot.ModSupport;
using Loot.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Loot.Api.Loaders
{
	/// <summary>
	/// The LoadingFunneler's purpose is to funnel all loading and unloading to one class
	/// </summary>
	internal static class LoadingFunneler
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
			RegistryLoader.ProcessMods();
			if (!Main.dedServ)
			{
				Loot.ModContentManager.Load();
			}
			LoadILEdits();
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
		}

		// Load EMM for Client only (this doesn't need to be loaded for server)
		private static void LoadModForClient()
		{
			LoadStaticAssets();

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
			RegistryLoader.Unload();
			Loot.ModContentManager?.Unload();
			Loot.ModContentManager = null;
			UnloadStaticAssets();
		}

		private static void LoadStaticAssets()
		{
			foreach (var mem in ReflectUtils.GetStaticAssetTypes())
			{
				var attr = (StaticAssetAttribute)mem.GetCustomAttribute(typeof(StaticAssetAttribute));
				Debug.Assert(attr != null);
				if (mem is PropertyInfo prop && prop.PropertyType == typeof(Texture2D))
					prop.SetValue(null, attr.LoadTexture2D());
				if (mem is FieldInfo field && field.FieldType == typeof(Texture2D))
					field.SetValue(null, attr.LoadTexture2D());
			}
		}

		private static void UnloadStaticAssets()
		{
			foreach (var mem in ReflectUtils.GetStaticAssetTypes())
			{
				if (!mem.GetType().IsValueType || Nullable.GetUnderlyingType(mem.GetType()) != null)
				{
					if (mem is PropertyInfo prop)
						prop.SetValue(null, null);
					if (mem is FieldInfo field)
						field.SetValue(null, null);
				}
			}
		}

		private static void LoadILEdits()
		{
			foreach (var type in ReflectUtils.GetILEdits())
			{
				// We do not need to store IL Edits after application: tModLoader auto unloads edits
				((ILEdit)Activator.CreateInstance(type)).Apply(Main.dedServ);
			}
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
