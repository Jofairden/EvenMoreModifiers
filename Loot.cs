using Loot.UI.Common.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using log4net;
using Loot.Api.Loaders;
using Loot.Api.ModContent;
using Loot.UI;
using Loot.UI.Tabs.Cubing;
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
		internal new static ILog Logger => ((Mod)Instance)?.Logger;
		internal static Loot Instance;

		internal UserInterface GuiInterface;
		internal GuiTabWindow GuiState;

		internal static ModContentManager ModContentManager;
		public static bool IsLoaded;

		public override void Load()
		{
			Instance = this;
			LoadingFunneler.Load();
		}

		public override void AddRecipes()
		{
			LoadingFunneler.PostLoad();
		}

		public override void Unload()
		{
			LoadingFunneler.Unload();
			Instance = null;
		}

		public override void PreSaveAndQuit()
		{
			// If we quit we must give back the item in slot if it's there
			GuiState.GetTab<GuiCubingTab>().GiveBackSlottedItem();
			if (GuiState.Visible)
			{
				GuiState.ToggleUI(GuiInterface);
			}
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
			}
		}
	}
}
