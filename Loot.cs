using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Loot.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

/*
 * original version by hiccup
 * reworked and maintained by jofairden
 * for tmodloader
 *
 * (c) Jofairden 2018
 */

namespace Loot
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	public sealed class Loot : Mod
	{
		internal static Loot Instance;

#if DEBUG
		public override string Name => "Loot";
#endif

		// 2 interfaces or 1?
		internal UserInterface CubeInterface;
		internal CubeRerollUI CubeRerollUI;
		internal CubeSealUI CubeSealUI;

		public Loot()
		{
			Properties = new ModProperties
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load()
		{
			Instance = this;

			//(string Name, string test) variable = ("Compiled with", "C#7");

			EMMLoader.Initialize();
			EMMLoader.Load();

			EMMLoader.RegisterMod(this);
			EMMLoader.SetupContent(this);

			if (!Main.dedServ)
			{
				CubeRerollUI = new CubeRerollUI();
				CubeRerollUI.Activate();

				CubeSealUI = new CubeSealUI();
				CubeSealUI.Activate();

				CubeInterface = new UserInterface();
			}
		}

		public override void Unload()
		{
			Instance = null;
			EMMLoader.Unload();

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
		}

		private GameTime _lastUpdateUIGameTime;
		public override void UpdateUI(GameTime gameTime)
		{
			_lastUpdateUIGameTime = gameTime;
			if (CubeInterface?.CurrentState != null) CubeInterface.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Loot: CubeUI",
					delegate
					{
						if (CubeInterface?.CurrentState is VisibilityUI 
						    && (CubeInterface.CurrentState as VisibilityUI).Visible 
						    && _lastUpdateUIGameTime != null)
						{
							CubeInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);
						}

						return true;
					},
					InterfaceScaleType.UI));
			}
		}

		// @todo: probably write our own handler for packets
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
		}
	}
}