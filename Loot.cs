using Loot.Core.ModContent;
using Loot.Core.System.Loaders;
using Loot.Ext.ModSupport;
using Loot.UI.Common;
using Loot.UI.Common.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

/*
 * original version by Hiccup
 * reworked and maintained by Jofairden
 * for tModLoader
 *
 * (c) Jofairden 2018
 */

[assembly: InternalsVisibleTo("LootTests")] // Allow doing unit tests

namespace Loot
{
	public sealed class Loot : Mod
	{
		internal static Loot Instance;

		internal UserInterface GuiInterface;
		internal GuiTabWindow GuiState;

		internal static ModContentManager ModContentManager;
		public static bool Loaded;

		public Loot()
		{
			Properties = new ModProperties
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public static void AddMod(Mod mod)
		{
			MainLoader.RegisterMod(mod);
			MainLoader.AddContent(mod);
		}

		public override void Load()
		{
			Instance = this;
			LoadMod();

			if (!Main.dedServ)
			{
				LoadModForClient();
			}
		}

		private void LoadMod()
		{
			ModSupport.Init();

			ContentLoader.Initialize();
			ContentLoader.Load();
			MainLoader.Initialize();
			MainLoader.Load();

			AddMod(this);

			ModSupport.AddServerSupport();
		}

		private void LoadModForClient()
		{
			SetupContentManager();
			SetupUserInterfaces();
			AssetLoader.RegisterAssets(this, "GraphicsAssets");
			ModSupport.AddClientSupport();
		}

		private void SetupContentManager()
		{
			ModContentManager = new ModContentManager();
			ModContentManager.Initialize(this);
		}

		private void SetupUserInterfaces()
		{
			GuiInterface = new UserInterface();
			GuiState = new GuiTabWindow();
			GuiState.Activate();
		}

		// AddRecipes() here functions as a PostLoad() hook where all mods have loaded
		public override void AddRecipes()
		{
			if (!Main.dedServ)
			{
				ModContentManager.Load();
			}

			Loaded = true;
		}

		public override void Unload()
		{
			Instance = null;

			ContentLoader.Unload();
			MainLoader.Unload();
			ModContentManager?.Unload();
			ModContentManager = null;

			// TODO causes trouble in unload?
			// @todo this is not a feature of tml
			// Attempt to unload our static variables
			//Stack<Type> typesToProcess = new Stack<Type>(this.Code.GetTypes());
			//while (typesToProcess.Count > 0)
			//{
			//    Type type = typesToProcess.Pop();
			//    foreach (FieldInfo info in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			//    {
			//        info.SetValue(null, info.FieldType.IsValueType ? Activator.CreateInstance(info.FieldType) : null);
			//    }
			//    foreach (Type nestedType in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			//    {
			//        typesToProcess.Push(nestedType);
			//    }
			//}

			Loaded = false;
		}

		// If we quit we must give back the item in slot if it's there
		public override void PreSaveAndQuit()
		{
			//if (CubeRerollUI._rerollItemPanel != null
			//	&& !CubeRerollUI._rerollItemPanel.item.IsAir)
			//{
			//	// Runs only in SP or client, so this is safe
			//	Main.LocalPlayer.QuickSpawnClonedItem(CubeRerollUI._rerollItemPanel.item, CubeRerollUI._rerollItemPanel.item.stack);
			//	CubeRerollUI._rerollItemPanel.item.TurnToAir();
			//}

			//if (CubeSealUI.SlottedItem != null
			//	&& !CubeSealUI.SlottedItem.IsAir)
			//{
			//	Main.LocalPlayer.QuickSpawnClonedItem(CubeSealUI.SlottedItem, CubeSealUI.SlottedItem.stack);
			//	CubeSealUI.SlottedItem.TurnToAir();
			//}
		}

		private GameTime _lastUpdateUiGameTime;

		public override void UpdateUI(GameTime gameTime)
		{
			_lastUpdateUiGameTime = gameTime;
			if (GuiInterface?.CurrentState != null)
			{
				GuiInterface.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Loot: GuiInterface",
					delegate
					{
						if (GuiInterface?.CurrentState is VisibilityUI visibilityUi
							&& visibilityUi.Visible && _lastUpdateUiGameTime != null)
						{
							GuiInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
						return true;
					},
					InterfaceScaleType.UI));

				//layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
				//	"Loot: CubeUI",
				//	delegate
				//	{
				//		if (((CubeInterface.CurrentState as CubeUI)?.Visible ?? false)
				//			&& _lastUpdateUIGameTime != null)
				//		{
				//			CubeInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);
				//		}

				//		return true;
				//	},
				//	InterfaceScaleType.UI));
			}
		}
	}
}
